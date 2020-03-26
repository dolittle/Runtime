// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the implementation of <see cref="EventHandlersBase"/>.
    /// </summary>
    public class EventHandlersService : EventHandlersBase
    {
        readonly IEventHandlers _eventHandlers;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="eventHandlers">The <see cref="IEventHandlers" />.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersService(
            IEventHandlers eventHandlers,
            IReverseCallDispatchers reverseCallDispatchers,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _eventHandlers = eventHandlers;
            _reverseCallDispatchers = reverseCallDispatchers;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override Task Connect(
            IAsyncStreamReader<EventHandlerClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            var sourceStream = StreamId.AllStreamId;
            var scope = ScopeId.Default;
            var eventHandlerArguments = context.GetArgumentsMessage<EventHandlerArguments>();
            var eventProcessorId = eventHandlerArguments.EventHandler.To<EventProcessorId>();
            var types = eventHandlerArguments.Types_.Select(_ => _.Id.To<ArtifactId>());
            var partitioned = eventHandlerArguments.Partitioned;
            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                runtimeStream,
                clientStream,
                context,
                _ => _.CallNumber,
                _ => _.CallNumber);
            var eventProcessor = new EventProcessor<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest>(
                scope,
                eventProcessorId,
                new EventHandlerProcessingRequestHandler<EventHandlerRuntimeToClientRequest, EventHandlerClientToRuntimeResponse>(
                    dispatcher,
                    response => response.ToProcessingResult()),
                _executionContextManager,
                (@event, partition, executionContext) => new EventHandlerProcessingRequestProxy(@event, partition, executionContext),
                _logger);
            return _eventHandlers.RegisterAndStartProcessing(
                scope,
                eventProcessorId,
                sourceStream,
                types,
                partitioned,
                dispatcher,
                eventProcessor,
                context.CancellationToken);
        }
    }
}