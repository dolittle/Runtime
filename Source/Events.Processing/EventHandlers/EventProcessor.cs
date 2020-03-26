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
        readonly IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> _dispatcher;
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
            IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> dispatcher,
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

        #nullable enable
        /// <inheritdoc />
        public async Task<IProcessingResult> Process(CommittedEvent @event, PartitionId partitionId, RetryProcessingState? retryProcessingState, CancellationToken cancellationToken = default)
        {
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' for partition '{partitionId.Value}'");

            var request = new EventHandlerRuntimeToClientRequest
                {
                    Event = @event.ToProtobuf(),
                    Partition = partitionId.ToProtobuf(),
                    ExecutionContext = _executionContextManager.Current.ToByteString(),
                    RetryProcessingState = retryProcessingState
                };

            ProcessingResult? result = null;
            await _dispatcher.Call(request, response => result = response).ConfigureAwait(false);
            return result!;
        }
    }
}