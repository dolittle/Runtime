// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Artifacts;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHandlers" />.
    /// </summary>
    public class EventHandlers : IEventHandlers
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IFilterRegistry> _getFilters;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly FactoryFor<IFetchEventsFromStreams> _eventsFromStreamsFetcherFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlers"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="getFilters">The <see cref="FactoryFor{T}"/> the <see cref="IFilterRegistry" />.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the  <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcherFactory"><see cref="FactoryFor{T}"/> the  <see cref="IFetchEventsFromStreams">fetcher</see> for writing events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public EventHandlers(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFilterRegistry> getFilters,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            FactoryFor<IFetchEventsFromStreams> eventsFromStreamsFetcherFactory,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _getFilters = getFilters;
            _streamProcessorsFactory = streamProcessorsFactory;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _eventsFromStreamsFetcherFactory = eventsFromStreamsFetcherFactory;
            _logger = logger;
        }

        /// <inheritdoc/>.
        public async Task RegisterAndStartProcessing<TResponse, TRequest>(
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStream,
            IEnumerable<ArtifactId> types,
            bool partitioned,
            IReverseCallDispatcher<TResponse, TRequest> dispatcher,
            IEventProcessor eventProcessor,
            CancellationToken cancellationToken)
            where TResponse : IMessage
            where TRequest : IMessage
        {
            try
            {
                _logger.Debug($"Registering event handler id '{eventProcessorId}' in scope '{scope}' for stream '{sourceStream}'");
                var targetStream = new StreamId { Value = eventProcessorId };

                ThrowIfIllegalTargetStream(targetStream);

                var filterDefinition = new TypeFilterWithEventSourcePartitionDefinition(
                    sourceStream,
                    targetStream,
                    types,
                    partitioned);
                await RegisterForAllTenants(filterDefinition, scope, eventProcessorId, sourceStream, targetStream, eventProcessor, cancellationToken).ConfigureAwait(false);
                await dispatcher.HandleCalls().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Error(ex, $"Error occurred while processing event handler '{eventProcessorId}' in scope '{scope}'");
                }

                throw;
            }
            finally
            {
                UnregisterForAllTenants(scope, eventProcessorId, sourceStream);
                _logger.Debug($"Event handler with id '{eventProcessorId}' in scope '{scope}' disconnected");
            }
        }

        async Task RegisterForAllTenants(
            TypeFilterWithEventSourcePartitionDefinition filterDefinition,
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStreamId,
            StreamId targetStreamId,
            IEventProcessor eventProcessor,
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

                    _streamProcessorsFactory().Register(filter, _eventsFromStreamsFetcherFactory(), sourceStreamId, cancellationToken);
                    _streamProcessorsFactory().Register(eventProcessor, _eventsFromStreamsFetcherFactory(), targetStreamId, cancellationToken);
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