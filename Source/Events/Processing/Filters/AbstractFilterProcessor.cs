// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Represents an implementation of <see cref="IEventProcessor" />that processes the filtering of an event.
/// </summary>
/// <typeparam name="TDefinition">The <see cref="IFilterDefinition" />.</typeparam>
public abstract class AbstractFilterProcessor<TDefinition> : IFilterProcessor<TDefinition>
    where TDefinition : IFilterDefinition
{
    readonly IWriteEventsToStreams _eventsToStreamsWriter;
    readonly ILogger _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractFilterProcessor{T}"/> class.
    /// </summary>
    /// <param name="scope">The <see cref="ScopeId" />.</param>
    /// <param name="filterDefinition">The <see typeparam="TDefinition"/> <see cref="IFilterDefinition" /> for the filter processor.</param>
    /// <param name="eventsToStreamsWriter">The <see cref="Func{TResult}" />.</param>
    /// <param name="logger"><see cref="ILogger" />.</param>
    protected AbstractFilterProcessor(
        ScopeId scope,
        TDefinition filterDefinition,
        IWriteEventsToStreams eventsToStreamsWriter,
        ILogger logger)
    {
        Scope = scope;
        Definition = filterDefinition;
        _eventsToStreamsWriter = eventsToStreamsWriter;
        _logger = logger;
    }

    /// <inheritdoc/>
    public ScopeId Scope { get; }

    /// <inheritdoc/>
    public TDefinition Definition { get; }

    /// <inheritdoc />
    public EventProcessorId Identifier => Definition.TargetStream.Value;

    public CancellationToken? ShutdownToken { get; } = null;
    public CancellationToken? DeadlineToken { get; } = null;

    /// <inheritdoc/>
    public abstract Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <inheritdoc/>
    public abstract Task<IFilterResult> Filter(CommittedEvent @event, PartitionId partitionId, EventProcessorId eventProcessorId, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.FilteringEvent(Identifier, Scope, @event.Type.Id, partitionId);
        var result = await Filter(@event, partitionId, Identifier, executionContext, cancellationToken).ConfigureAwait(false);

        return await HandleResult(result, @event, partitionId, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, StreamPosition position, string failureReason, uint retryCount, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.FilteringEventAgain(Identifier, Scope, @event.Type.Id, partitionId, retryCount, failureReason);
        var result = await Filter(@event, partitionId, Identifier, failureReason, retryCount, executionContext, cancellationToken).ConfigureAwait(false);

        return await HandleResult(result, @event, partitionId, cancellationToken).ConfigureAwait(false);
    }

    async Task<IProcessingResult> HandleResult(IFilterResult result, CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
    {
        _logger.HandleFilterResult(Identifier, Scope, @event.Type.Id, partitionId);

        if (!result.Succeeded)
        {
            _logger.FailedToFilterEvent(Identifier, Scope, @event.Type.Id);
            return result;
        }

        if (!result.IsIncluded)
        {
            return result;
        }

        _logger.FilteredEventIsIncluded(Identifier, Scope, @event.Type.Id, partitionId, Definition.TargetStream);
        try
        {
            await _eventsToStreamsWriter
                .Write(@event, Scope, Definition.TargetStream, result.Partition, cancellationToken)
                .ConfigureAwait(false);
            return result;
        }
        catch (EventLogSequenceAlreadyWritten ex)
        {
            _logger.LogWarning("Event {Event} was already written to stream {Stream}", ex.EventLogSequenceNumber, Definition.TargetStream);
            return result; // No need to retry, already written
        }
        catch (Exception ex)
        {
            return new FailedFiltering($"Failed to write filtered event to stream {ex}", true, TimeSpan.FromSeconds(10));
        }
    }
}
