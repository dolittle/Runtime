// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventProcessor" />that processes the handling of an event.
    /// </summary>
    public class EventProcessor : IEventProcessor
    {
        readonly IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> _callDispatcher;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly string _logMessagePrefix;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessor"/> class.
        /// </summary>
        /// <param name="id">The <see cref="EventProcessorId" />.</param>
        /// <param name="callDispatcher"><see cref="IReverseCallDispatcher{TResponse, TRequest}"/> for server requests.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventProcessor(
            EventProcessorId id,
            IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> callDispatcher,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _callDispatcher = callDispatcher;
            _executionContextManager = executionContextManager;
            Identifier = id;
            _logger = logger;
            _logMessagePrefix = $"Event Processor '{Identifier}'";
        }

        /// <inheritdoc />
        public EventProcessorId Identifier { get; }

        /// <inheritdoc />
        public async Task<IProcessingResult> Process(Store.CommittedEvent @event, PartitionId partitionId)
        {
            _logger.Debug($"{_logMessagePrefix} is processing event '{@event.Type.Id.Value}' for partition '{partitionId.Value}'");

            var message = new EventHandlerRuntimeToClientRequest
            {
                Event = @event.ToProtobuf(),
                ExecutionContext = _executionContextManager.Current.ToByteString()
            };

            await _callDispatcher.Call(message, _ => { }).ConfigureAwait(false);

            return new SucceededProcessingResult();
        }
    }
}