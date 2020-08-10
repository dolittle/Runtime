// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly IReverseCallDispatcher<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> _dispatcher;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

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
            IReverseCallDispatcher<EventHandlerClientToRuntimeMessage, EventHandlerRuntimeToClientMessage, EventHandlerRegistrationRequest, EventHandlerRegistrationResponse, HandleEventRequest, EventHandlerResponse> dispatcher,
            ILogger logger)
        {
            Scope = scope;
            Identifier = id;
            _dispatcher = dispatcher;
            _logger = logger;
            _logMessagePrefix = $"Event Processor '{Identifier}'";
        }

        /// <inheritdoc />
        public ScopeId Scope { get; }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, CancellationToken cancellationToken)
        {
            _logger.Debug(
                "{LogMessagePrefix} is processing event '{EventTypeId}' for partition '{PartitionId}'",
                _logMessagePrefix,
                @event.Type.Id.Value,
                partitionId.Value);

            var request = new HandleEventRequest
                {
                    Event = new Contracts.StreamEvent { Event = @event.ToProtobuf(), PartitionId = partitionId.ToProtobuf(), ScopeId = Scope.ToProtobuf() },
                };
            return Process(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.Debug(
                "{LogMessagePrefix} is processing event '{EventTypeId}' for partition '{PartitionId}' again for the {RetryCount}. time because: {FailureReason}",
                _logMessagePrefix,
                @event.Type.Id.Value,
                partitionId.Value,
                retryCount,
                failureReason);
            var request = new HandleEventRequest
                {
                    Event = new Contracts.StreamEvent { Event = @event.ToProtobuf(), PartitionId = partitionId.ToProtobuf(), ScopeId = Scope.ToProtobuf() },
                    RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
                };
            return Process(request, cancellationToken);
        }

        async Task<IProcessingResult> Process(HandleEventRequest request, CancellationToken cancellationToken)
        {
            var response = await _dispatcher.Call(request, cancellationToken).ConfigureAwait(false);

            return response switch
                {
                    { Failure: null } => new SuccessfulProcessing(),
                    _ => new FailedProcessing(response.Failure.Reason, response.Failure.Retry, response.Failure.RetryTimeout.ToTimeSpan())
                };
        }
    }
}
