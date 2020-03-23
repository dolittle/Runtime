// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading;
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
using Dolittle.Runtime.Events.Store;
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
        readonly FactoryFor<IFilterRegistry> _getFilters;
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
        /// <param name="getFilters">The <see cref="FactoryFor{T}"/> the <see cref="IFilterRegistry" />.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the  <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcherFactory"><see cref="FactoryFor{T}"/> the  <see cref="IFetchEventsFromStreams">fetcher</see> for writing events.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlersService(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFilterRegistry> getFilters,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            FactoryFor<IFetchEventsFromStreams> eventsFromStreamsFetcherFactory,
            IReverseCallDispatchers reverseCallDispatchers,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _getFilters = getFilters;
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
            var sourceStream = StreamId.AllStreamId;
            var scope = ScopeId.Default;

            try
            {
                var eventHandlerArguments = context.GetArgumentsMessage<EventHandlerArguments>();
                eventProcessorId = eventHandlerArguments.EventHandler.To<EventProcessorId>();
                _logger.Debug($"EventHandler client connected with id '{eventProcessorId}' for stream '{sourceStream}'");
                var targetStream = new StreamId { Value = eventProcessorId };

                ThrowIfIllegalTargetStream(targetStream);

                var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                    runtimeStream,
                    clientStream,
                    context,
                    _ => _.CallNumber,
                    _ => _.CallNumber);
                var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(
                    sourceStream,
                    targetStream,
                    eventHandlerArguments.Types_.Select(_ => _.Id.To<ArtifactId>()),
                    eventHandlerArguments.Partitioned);
                await RegisterForAllTenants(filterDefinition, dispatcher, scope, eventProcessorId, sourceStream, targetStream, context.CancellationToken).ConfigureAwait(false);
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
                UnregisterForAllTenants(scope, eventProcessorId, sourceStream);
                _logger.Debug($"EventHandler client disconnected for '{eventProcessorId}'");
            }
        }

        async Task RegisterForAllTenants(
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            IReverseCallDispatcher<EventHandlerClientToRuntimeResponse, EventHandlerRuntimeToClientRequest> callDispatcher,
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStreamId,
            StreamId targetStreamId,
            CancellationToken cancellationToken)
        {
            _logger.Debug($"Registering event handler '{eventProcessorId}' in scope '{scope}' for stream '{sourceStreamId}' for {_tenants.All.Count} tenants - types : '{string.Join(",", filterDefinition.Types)}'");
            foreach (var tenant in _tenants.All)
            {
                try
                {
                    _executionContextManager.CurrentFor(tenant);
                    var filter = new TypeFilterWithEventSourcePartition(
                                        scope,
                                        filterDefinition,
                                        _eventsToStreamsWriterFactory(),
                                        _logger);
                    await _getFilters().Register(filter, cancellationToken).ConfigureAwait(false);

                    _streamProcessorsFactory().Register(filter, _eventsFromStreamsFetcherFactory(), sourceStreamId);

                    var eventProcessor = new EventProcessor(
                        scope,
                        eventProcessorId,
                        callDispatcher,
                        _executionContextManager,
                        _logger);

                    _streamProcessorsFactory().Register(eventProcessor, _eventsFromStreamsFetcherFactory(), targetStreamId);
                }
                catch (IllegalFilterTransformation ex)
                {
                    _logger.Error(ex, $"The filter for stream '{targetStreamId}' in scope '{scope}' for tenant '{tenant}' does not produce the same stream as the previous filter for that stream. Not registering stream processors.");
                }
            }
        }

        void UnregisterForAllTenants(ScopeId scope, EventProcessorId eventProcessorId, StreamId sourceStream)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Unregistering filter '{eventProcessorId}' in scope '{scope}' for stream '{sourceStream}' for {tenants.Count} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _getFilters().Unregister(scope, eventProcessorId.Value);
                _streamProcessorsFactory().Unregister(scope, eventProcessorId, sourceStream);
                _streamProcessorsFactory().Unregister(scope, eventProcessorId, eventProcessorId.Value);
            });
        }

        void ThrowIfIllegalTargetStream(StreamId stream)
        {
            if (stream.IsNonWriteable) throw new CannotFilterToNonWriteableStream(stream);
        }
    }
}