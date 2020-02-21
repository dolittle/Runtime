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
        readonly ITypePartitionFilterRegistry _typePartitionFilterRegistry;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly FactoryFor<IFetchEventsFromStreams> _eventsFromStreamsFetcherFactory;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="typePartitionFilterRegistry">The <see cref="ITypePartitionFilterRegistry"/>.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the  <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcherFactory"><see cref="FactoryFor{T}"/> the  <see cref="IFetchEventsFromStreams">fetcher</see> for writing events.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersService(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            ITypePartitionFilterRegistry typePartitionFilterRegistry,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            FactoryFor<IFetchEventsFromStreams> eventsFromStreamsFetcherFactory,
            IReverseCallDispatchers reverseCallDispatchers,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _typePartitionFilterRegistry = typePartitionFilterRegistry;
            _streamProcessorsFactory = streamProcessorsFactory;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _eventsFromStreamsFetcherFactory = eventsFromStreamsFetcherFactory;
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
            StreamId sourceStreamId = Guid.Empty;

            try
            {
                var eventHandlerArguments = context.GetArgumentsMessage<EventHandlerArguments>();
                eventProcessorId = eventHandlerArguments.EventHandlerId.To<EventProcessorId>();
                sourceStreamId = eventHandlerArguments.StreamId.To<StreamId>();
                _logger.Debug($"EventHandler client connected with id '{eventProcessorId}' for stream '{sourceStreamId}'");
                var targetStreamId = new StreamId { Value = eventProcessorId };

                ThrowIfIllegalTargetStream(targetStreamId);

                var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                    runtimeStream,
                    clientStream,
                    context,
                    _ => _.CallNumber,
                    _ => _.CallNumber);
                var filterDefinition = await CreateAndValidateTypePartitionFilterDefinition(eventHandlerArguments, eventProcessorId, targetStreamId).ConfigureAwait(false);
                RegisterStreamProcessorsForAllTenants(filterDefinition, dispatcher, eventProcessorId, sourceStreamId, targetStreamId);
                await RegisterTypePartitionFilterDefinitions(eventProcessorId, targetStreamId, filterDefinition).ConfigureAwait(false);
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
                UnregisterForAllTenants(eventProcessorId, sourceStreamId);
                _logger.Debug($"EventHandler client disconnected for '{eventProcessorId}'");
            }
        }

        async Task<TypeFilterWithEventSourcePartitionDefinition> CreateAndValidateTypePartitionFilterDefinition(EventHandlerArguments eventHandlerArguments, EventProcessorId eventProcessor, StreamId sourceStream)
        {
            var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(
                eventHandlerArguments.Types_.Select(_ => _.Id.To<ArtifactId>()),
                eventHandlerArguments.Partitioned);

            foreach (var tenant in _tenants.All)
            {
                _executionContextManager.CurrentFor(tenant);
                await _typePartitionFilterRegistry.Validate(eventProcessor.Value, sourceStream, filterDefinition).ConfigureAwait(false);
            }

            return filterDefinition;
        }

        void RegisterStreamProcessorsForAllTenants(
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> callDispatcher,
            EventProcessorId eventProcessorId,
            StreamId sourceStreamId,
            StreamId targetStreamId)
        {
            _logger.Debug($"Registering event handler '{eventProcessorId}' for stream '{sourceStreamId}' for {_tenants.All.Count()} tenants - types : '{string.Join(",", filterDefinition.Types)}'");
            _tenants.All.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);

                var filter = new TypeFilterWithEventSourcePartition(
                                    eventProcessorId,
                                    targetStreamId,
                                    filterDefinition,
                                    _eventsToStreamsWriterFactory(),
                                    _logger);

                _streamProcessorsFactory().Register(filter, _eventsFromStreamsFetcherFactory(), sourceStreamId);

                var eventProcessor = new EventProcessor(
                    eventProcessorId,
                    callDispatcher,
                    _executionContextManager,
                    _logger);

                _streamProcessorsFactory().Register(eventProcessor, _eventsFromStreamsFetcherFactory(), targetStreamId);
            });
        }

        async Task RegisterTypePartitionFilterDefinitions(EventProcessorId eventProcessor, StreamId sourceStream, TypeFilterWithEventSourcePartitionDefinition filterDefinition)
        {
            foreach (var tenant in _tenants.All)
            {
                _executionContextManager.CurrentFor(tenant);
                await _typePartitionFilterRegistry.Register(eventProcessor.Value, sourceStream, filterDefinition).ConfigureAwait(false);
            }
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

        void ThrowIfIllegalTargetStream(StreamId stream)
        {
            if (stream.IsNonWriteable) throw new CannotFilterToNonWriteableStream(stream);
        }
    }
}