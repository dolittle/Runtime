// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using System.Linq;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents an implementation of <see cref="IEventProcessor" />that processes the projection from an event.
/// </summary>
public class EventProcessor : IEventProcessor
{
    readonly ProjectionDefinition _projectionDefinition;
    readonly IProjectionPersister _projectionPersister;
    readonly IProjectionStore _projectionStore;
    readonly IProjectionKeys _projectionKeys;
    readonly IProjection _projection;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="EventProcessor"/> class.
    /// </summary>
    /// <param name="projectionDefinition">The <see cref="ProjectionDefinition" />.</param>
    /// <param name="projectionPersister">The <see cref="IProjectionPersister"/>.</param>
    /// <param name="projectionStore">The <see cref="IProjectionStore" />.</param>
    /// <param name="projectionKeys">The <see cref="IProjectionKeys" />.</param>
    /// <param name="projection">The <see cref="IProjection" />.</param>
    /// /// <param name="logger">The <see cref="ILogger" />.</param>
    public EventProcessor(
        ProjectionDefinition projectionDefinition,
        IProjectionPersister projectionPersister,
        IProjectionStore projectionStore,
        IProjectionKeys projectionKeys,
        IProjection projection,
        ILogger logger)
    {
        Scope = projectionDefinition.Scope;
        Identifier = projectionDefinition.Projection.Value;
        _projectionDefinition = projectionDefinition;
        _projectionPersister = projectionPersister;
        _projectionStore = projectionStore;
        _projectionKeys = projectionKeys;
        _projection = projection;
        _logger = logger;
    }

    /// <inheritdoc />
    public ScopeId Scope { get; }

    /// <inheritdoc />
    public EventProcessorId Identifier { get; }

    /// <inheritdoc />
    public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
    {
        Log.EventProcessorIsProcessing(_logger, Identifier, @event.Type.Id, partitionId);
        if (!ShouldProcessEvent(@event))
        {
            return new SuccessfulProcessing();
        }

        var tryGetCurrentState = await TryGetCurrentState(@event, partitionId, cancellationToken).ConfigureAwait(false);
        if (!tryGetCurrentState.Success)
        {
            return new FailedProcessing(tryGetCurrentState.Exception.Message);
        }

        var result = await _projection.Project(tryGetCurrentState.Result, @event, partitionId, cancellationToken).ConfigureAwait(false);

        return await HandleResult(tryGetCurrentState.Result.Key, result, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
    {
        Log.EventProcessorIsProcessingAgain(_logger, Identifier, @event.Type.Id, partitionId, retryCount, failureReason);
        if (!ShouldProcessEvent(@event))
        {
            return new SuccessfulProcessing();
        }


        var tryGetCurrentState = await TryGetCurrentState(@event, partitionId, cancellationToken).ConfigureAwait(false);
        if (!tryGetCurrentState.Success)
        {
            return new FailedProcessing(tryGetCurrentState.Exception.Message);
        }

        var result = await _projection.Project(tryGetCurrentState.Result, @event, partitionId, failureReason, retryCount, cancellationToken).ConfigureAwait(false);

        return await HandleResult(tryGetCurrentState.Result.Key, result, cancellationToken).ConfigureAwait(false);
    }

    bool ShouldProcessEvent(CommittedEvent @event)
        => _projectionDefinition.Events.Select(_ => _.EventType).Contains(@event.Type.Id);

    async Task<Try<ProjectionCurrentState>> TryGetCurrentState(CommittedEvent @event, PartitionId partitionId, CancellationToken token)
    {
        if (!_projectionKeys.TryGetFor(_projectionDefinition, @event, partitionId, out var projectionKey))
        {
            Log.CouldNotGetProjectionKey(_logger, Identifier, Scope, @event.EventLogSequenceNumber);
            return new CouldNotGetProjectionKey(@event);
        }
        return await _projectionStore.TryGet(_projectionDefinition.Projection, Scope, projectionKey, token).ConfigureAwait(false);
    }

    async Task<IProcessingResult> HandleResult(ProjectionKey key, IProjectionResult result, CancellationToken token)
    {
        if (result is ProjectionReplaceResult replace)
        {
            return await _projectionPersister.TryReplace(_projectionDefinition, key, replace.State, token).ConfigureAwait(false) switch
            {
                true => new SuccessfulProcessing(),
                false => new FailedProcessing($"Failed to replace state for projection {_projectionDefinition.Projection.Value} with key {key.Value}"),
            };
        }
        else if (result is ProjectionDeleteResult)
        {
            return await _projectionPersister.TryRemove(_projectionDefinition, key, token).ConfigureAwait(false) switch
            {
                true => new SuccessfulProcessing(),
                false => new FailedProcessing($"Failed to remove state for projection {_projectionDefinition.Projection.Value} with key {key.Value}"),
            };
        }
        else if (result is ProjectionFailedResult failed)
        {
            return new FailedProcessing(failed.Exception.Message);
        }

        return new FailedProcessing($"Unknown projection result {result.GetType().Name}");
    }
}
