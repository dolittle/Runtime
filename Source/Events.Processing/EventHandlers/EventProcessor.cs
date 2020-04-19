// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly IReverseCallDispatcher<EventHandlersClientToRuntimeStreamMessage, EventHandlerRuntimeToClientStreamMessage> _dispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="scope">The <see cref="ScopeId" />.</param>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="dispatcher"><see cref="IReverseCallDispatcher{TRequest, TResponse}"/> dispatcher.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            ScopeId scope,
            EventProcessorId id,
            IReverseCallDispatcher<EventHandlersClientToRuntimeStreamMessage, EventHandlerRuntimeToClientStreamMessage> dispatcher,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            Scope = scope;
            Identifier = id;
            _dispatcher = dispatcher;
            _executionContextManager = executionContextManager;
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
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' for partition '{partitionId.Value}'");

            var request = new HandleEventRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf()
                };
            return Process(request, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, string failureReason, uint retryCount, CancellationToken cancellationToken)
        {
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' for partition '{partitionId.Value}' again for the {retryCount}. time because: {failureReason}");
            var request = new HandleEventRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                    RetryProcessingState = new RetryProcessingState { FailureReason = failureReason, RetryCount = retryCount }
                };
            return Process(request, cancellationToken);
        }

#pragma warning disable CA1801
        async Task<IProcessingResult> Process(HandleEventRequest request, CancellationToken cancellationToken)
        {
            IProcessingResult result = null;
            await _dispatcher.Call(
                new EventHandlerRuntimeToClientStreamMessage { HandleRequest = request, ExecutionContext = _executionContextManager.Current.ToByteString() },
                response =>
                {
                    if (response.MessageCase != EventHandlersClientToRuntimeStreamMessage.MessageOneofCase.HandleResult)
                    {
                        result = response.HandleResult switch
                            {
                                { Failed: null } => new SuccessfulProcessing(),
                                _ => new FailedProcessing(response.HandleResult.Failed.Reason, response.HandleResult.Failed.Retry, response.HandleResult.Failed.RetryTimeout.ToTimeSpan())
                            };
                    }
                    else
                    {
                        result = new FailedProcessing("The response from the processing was of an unexpected response type.");
                    }
                }).ConfigureAwait(false);
            return result!;
        }
#pragma warning restore CA1801
    }
}
