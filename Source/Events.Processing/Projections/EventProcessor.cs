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

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the projection from an event.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> _dispatcher;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> dispatcher.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ScopeId scope,
            EventProcessorId id,
            IReverseCallDispatcher<ProjectionClientToRuntimeMessage, ProjectionRuntimeToClientMessage, ProjectionRegistrationRequest, ProjectionRegistrationResponse, ProjectionRequest, ProjectionResponse> dispatcher,
            ILogger logger)
        {
            Scope = scope;
            Identifier = id;
            _dispatcher = dispatcher;
            _logger = logger;
        }

        /// <inheritdoc />
        public ScopeId Scope { get; }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.EventProcessorIsProcessing(Identifier, @event.Type.Id, partitionId);

            var request = new ProjectionRequest
            {
                CurrentState = GetCurrentState(),
                Event = CreateStreamEvent(@event, partitionId)
            };

            return Process(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.EventProcessorIsProcessingAgain(Identifier, @event.Type.Id, partitionId, retryCount, failureReason);
            var request = new ProjectionRequest
            {
                CurrentState = GetCurrentState(),
                Event = CreateStreamEvent(@event, partitionId),
                RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
            };
            return Process(request, cancellationToken);
        }

        async Task<IProcessingResult> Process(ProjectionRequest request, CancellationToken cancellationToken)
        {
            var response = await _dispatcher.Call(request, cancellationToken).ConfigureAwait(false);

            return response switch
            {
                { Failure: null } => new SuccessfulProcessing(),
                _ => new FailedProcessing(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout.ToTimeSpan())
            };
        }

        ProjectionCurrentState GetCurrentState() => new() { Type = Contracts.ProjectionCurrentStateType.CreatedFromInitialState, State = string.Empty };

        Contracts.StreamEvent CreateStreamEvent(CommittedEvent @event, PartitionId partitionId)
            => new() { Event = @event.ToProtobuf(), PartitionId = partitionId.ToProtobuf(), ScopeId = Scope.ToProtobuf() };
    }
}
