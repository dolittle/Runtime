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
        public override async Task Connect(
            IAsyncStreamReader<EventHandlersClientToRuntimeStreamMessage> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientStreamMessage> clientStream,
            ServerCallContext context)
        {
            if (!await runtimeStream.MoveNext(context.CancellationToken).ConfigureAwait(false))
            {
                const string message = "EventHandlers connection requested but client-to-runtime stream did not contain any messages";
                _logger.Warning(message);
                await clientStream.WriteAsync(new EventHandlerRuntimeToClientStreamMessage
                    {
                        RegistrationResponse = new EventHandlerRegistrationResponse
                        {
                            Failure = new Failure { Reason = message }
                        }
                    }).ConfigureAwait(false);
                return;
            }

            if (runtimeStream.Current.MessageCase != EventHandlersClientToRuntimeStreamMessage.MessageOneofCase.RegistrationRequest)
            {
                const string message = "EventHandlers connection requested but first message in request stream was not an event handler registration request message";
                _logger.Warning(message);
                await clientStream.WriteAsync(new EventHandlerRuntimeToClientStreamMessage
                    {
                        RegistrationResponse = new EventHandlerRegistrationResponse
                        {
                            Failure = new Failure { Reason = $"The first message in the event handler connection needs to be {typeof(EventHandlersRegistrationRequest).FullName}" }
                        }
                    }).ConfigureAwait(false);
                return;
            }

            var sourceStream = StreamId.AllStreamId;
            var registration = runtimeStream.Current.RegistrationRequest;
            var eventProcessorId = registration.EventHandler.To<EventProcessorId>();
            var scope = registration.Scope.To<ScopeId>();
            var types = registration.Types_.Select(_ => _.Id.To<ArtifactId>());
            var partitioned = registration.Partitioned;

            var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                runtimeStream,
                clientStream,
                context,
                _ => _.CallNumber,
                _ => _.CallNumber);
            var eventProcessor = new EventProcessor(
                scope,
                eventProcessorId,
                dispatcher,
                _executionContextManager,
                _logger);
            await _eventHandlers.RegisterAndStartProcessing(
                scope,
                eventProcessorId,
                sourceStream,
                types,
                partitioned,
                dispatcher,
                eventProcessor,
                context.CancellationToken).ConfigureAwait(false);
        }
    }
}