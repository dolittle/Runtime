// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the projection from an event.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly ProjectionDefinition _projectionDefinition;
        readonly IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> _dispatcher;
        readonly IProjectionStates _projectionStates;
        readonly IProjectionKeys _projectionKeys;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="projectionDefinition">The <see cref="ProjectionDefinition" />.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> dispatcher.</param>
        /// <param name="projectionStates">The <see cref="IProjectionStates" />.</param>
        /// <param name="projectionKeys">The <see cref="IProjectionKeys" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ProjectionDefinition projectionDefinition,
            IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> dispatcher,
            IProjectionStates projectionStates,
            IProjectionKeys projectionKeys,
            ILogger logger)
        {
            Scope = projectionDefinition.Scope;
            Identifier = projectionDefinition.Projection.Value;
            _projectionDefinition = projectionDefinition;
            _projectionStates = projectionStates;
            _dispatcher = dispatcher;
            _projectionKeys = projectionKeys;
            _logger = logger;
        }

        /// <inheritdoc />
        public ScopeId Scope { get; }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.EventProcessorIsProcessing(Identifier, @event.Type.Id, partitionId);

            return await Process(@event, partitionId, new(), cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.EventProcessorIsProcessingAgain(Identifier, @event.Type.Id, partitionId, retryCount, failureReason);
            var request = new ProjectionRequest
            {
                RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
            };
            return await Process(@event, partitionId, request, cancellationToken).ConfigureAwait(false);
        }

        async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, ProjectionRequest request, CancellationToken token)
        {
            if (!_projectionKeys.TryGetFor(_projectionDefinition, @event, partitionId, out var projectionKey))
            {
                _logger.CouldNotGetProjectionKey(Identifier, Scope);
                return new FailedProcessing("Could not get projection key");
            }

            request.Event = CreateStreamEvent(@event, partitionId);
            request.Key = projectionKey;
            request.CurrentState = await GetCurrentState(@projectionKey, token).ConfigureAwait(false);

            var response = await _dispatcher.Call(request, token).ConfigureAwait(false);
            return await (response switch
            {
                { Failure: null } => HandleResponse(projectionKey, response.NextState, token),
                _ => Task.FromResult<IProcessingResult>(new FailedProcessing(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout.ToTimeSpan()))
            }).ConfigureAwait(false);
        }

        async Task<ProjectionCurrentState> GetCurrentState(ProjectionKey projectionKey, CancellationToken token)
        {
            var tryGetState = await _projectionStates.TryGet(_projectionDefinition.Projection, Scope, projectionKey, token).ConfigureAwait(false);
            return tryGetState.Success switch
            {
                true => new ProjectionCurrentState { Type = ProjectionCurrentStateType.Persisted, State = tryGetState.Result },
                false => new ProjectionCurrentState { Type = ProjectionCurrentStateType.CreatedFromInitialState, State = _projectionDefinition.InititalState },
            };
        }

        async Task<IProcessingResult> HandleResponse(ProjectionKey key, ProjectionNextState nextState, CancellationToken cancellationToken)
        {
            var successfulUpdate = await (nextState.Type switch
            {
                ProjectionNextStateType.Replace => TryReplace(key, nextState.Value, cancellationToken),
                ProjectionNextStateType.Delete => TryRemove(key, cancellationToken),
                _ => Task.FromResult(false)
            }).ConfigureAwait(false);

            return successfulUpdate switch
            {
                true => new SuccessfulProcessing(),
                false => new FailedProcessing("Failed to update state for projection")
            };
        }


        async Task<bool> TryReplace(ProjectionKey key, ProjectionState newState, CancellationToken token)
            => await _projectionStates.TryReplace(_projectionDefinition.Projection, Scope, key, newState, token).ConfigureAwait(false);

        async Task<bool> TryRemove(ProjectionKey key, CancellationToken token)
            => await _projectionStates.TryRemove(_projectionDefinition.Projection, Scope, key, token).ConfigureAwait(false);

        Contracts.StreamEvent CreateStreamEvent(CommittedEvent @event, PartitionId partitionId)
            => new() { Event = @event.ToProtobuf(), PartitionId = partitionId.ToProtobuf(), ScopeId = Scope.ToProtobuf() };
    }
}