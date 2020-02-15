// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the implementation of <see cref="EventHandlersBase"/>.
    /// </summary>
    public class EventHandlersService : EventHandlersBase
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the  <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersService(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            IReverseCallDispatchers reverseCallDispatchers,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _streamProcessorsFactory = streamProcessorsFactory;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _reverseCallDispatchers = reverseCallDispatchers;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<EventHandlerClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<EventHandlerRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            EventProcessorId eventProcessorId = Guid.Empty;
            StreamId streamId = Guid.Empty;

            try
            {
                var eventHandlerArguments = context.GetArgumentsMessage<EventHandlerArguments>();
                eventProcessorId = eventHandlerArguments.EventHandlerId.To<EventProcessorId>();
                streamId = eventHandlerArguments.StreamId.To<StreamId>();

                _logger.Debug($"EventHandler client connected with id '{eventProcessorId}' for stream '{streamId}'");

                var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                    runtimeStream,
                    clientStream,
                    context,
                    _ => _.CallNumber,
                    _ => _.CallNumber);

                RegisterForAllTenants(eventHandlerArguments, dispatcher, eventProcessorId, streamId);

                await dispatcher.WaitTillDisconnected().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Error(ex, $"Error occurred while processing event handler '{eventProcessorId}'");
                }
            }
            finally
            {
                UnregisterForAllTenants(eventProcessorId, streamId);
                _logger.Debug($"EventHandler client disconnected for '{eventProcessorId}'");
            }
        }

        void RegisterForAllTenants(
            EventHandlerArguments eventHandlerArguments,
            IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> callDispatcher,
            EventProcessorId eventProcessorId,
            StreamId sourceStreamId)
        {
            var tenants = _tenants.All;
            var targetStreamId = (StreamId)eventProcessorId.Value;

            var definition = new TypeFilterWithEventSourcePartitionDefinition(
                eventHandlerArguments.Types_.Select(_ => _.Id.To<ArtifactId>()),
                eventHandlerArguments.Partitioned);

            _logger.Debug($"Registering event handler '{eventProcessorId}' for stream '{sourceStreamId}' for {tenants.Count()} tenants - types : '{string.Join(",", definition.Types)}'");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);

                var filter = new TypeFilterWithEventSourcePartition(
                                    eventProcessorId,
                                    targetStreamId,
                                    definition,
                                    _eventsToStreamsWriterFactory(),
                                    _logger);

                _streamProcessorsFactory().Register(filter, sourceStreamId);

                var eventProcessor = new EventProcessor(
                    eventProcessorId,
                    callDispatcher,
                    _executionContextManager,
                    _logger);

                _streamProcessorsFactory().Register(eventProcessor, targetStreamId);
            });
        }

        void UnregisterForAllTenants(EventProcessorId eventProcessorId, StreamId sourceStreamId)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Unregistering filter '{eventProcessorId}' for stream '{sourceStreamId}' for {tenants.Count()} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _streamProcessorsFactory().Unregister(eventProcessorId, sourceStreamId);
                _streamProcessorsFactory().Unregister(eventProcessorId, eventProcessorId.Value);
            });
        }
    }
}