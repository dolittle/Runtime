// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Google.Protobuf;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilters" />.
    /// </summary>
    public class Filters : IFilters
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IFilterRegistry> _filterRegistryFactory;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly FactoryFor<IFetchEventsFromStreams> _eventsFromStreamsFetcherFactory;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="Filters"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="filterRegistryFactory">The <see cref="FactoryFor{T}" /> the <see cref="IFilterRegistry" />.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcherFactory"><see cref="FactoryFor{T}"/> the <see cref="IFetchEventsFromStreams">fetcher</see> for fetching events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public Filters(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFilterRegistry> filterRegistryFactory,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            FactoryFor<IFetchEventsFromStreams> eventsFromStreamsFetcherFactory,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _filterRegistryFactory = filterRegistryFactory;
            _streamProcessorsFactory = streamProcessorsFactory;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _logger = logger;
            _eventsFromStreamsFetcherFactory = eventsFromStreamsFetcherFactory;
        }

        /// <inheritdoc/>
        public async Task RegisterAndStartProcessing<TResponse, TRequest, TFilterDefinition>(
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId sourceStream,
            IReverseCallDispatcher<TResponse, TRequest> dispatcher,
            IFilterProcessor<TFilterDefinition> filter,
            CancellationToken cancellationToken)
            where TResponse : IMessage
            where TRequest : IMessage
            where TFilterDefinition : IFilterDefinition
        {
            var streamId = new StreamId { Value = eventProcessorId };
            try
            {
                ThrowIfIllegalTargetStream(streamId);
                _logger.Debug($"Registering filter '{eventProcessorId}' in scope '{scope}'");

                await RegisterForAllTenants(scope, eventProcessorId, streamId, filter, cancellationToken).ConfigureAwait(false);

                await dispatcher.WaitTillDisconnected().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.Error(ex, $"Error occurred while handling filter client '{eventProcessorId}'");
                }

                throw;
            }
            finally
            {
                UnregisterForAllTenants(scope, eventProcessorId, streamId);
                _logger.Debug($"Filter client disconnected - '{eventProcessorId}'");
            }
        }

        async Task RegisterForAllTenants<TFilterDefinition>(
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId streamId,
            IFilterProcessor<TFilterDefinition> filter,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            _logger.Debug($"Registering filter '{eventProcessorId}' in scope '{scope}' for stream for {_tenants.All.Count} tenants");

            foreach (var tenant in _tenants.All)
            {
                try
                {
                    _executionContextManager.CurrentFor(tenant);
                    await _filterRegistryFactory().Register(filter, cancellationToken).ConfigureAwait(false);
                    _streamProcessorsFactory().Register(filter, _eventsFromStreamsFetcherFactory(), streamId);
                }
                catch (IllegalFilterTransformation ex)
                {
                    _logger.Error(ex, $"The filter for stream '{eventProcessorId}' in scope '{scope}' for tenant '{tenant}' does not produce the same stream as the previous filter for that stream. Not registering stream processors.");
                }
            }
        }

        void UnregisterForAllTenants(ScopeId scope, EventProcessorId eventProcessorId, StreamId streamId)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Unregistering filter '{eventProcessorId}' in scope '{scope}' on stream '{streamId}' for {tenants.Count} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _filterRegistryFactory().Unregister(scope, eventProcessorId.Value);
                _streamProcessorsFactory().Unregister(scope, eventProcessorId, streamId);
            });
        }

        void ThrowIfIllegalTargetStream(StreamId stream)
        {
            if (stream.IsNonWriteable) throw new CannotFilterToNonWriteableStream(stream);
        }
    }
}