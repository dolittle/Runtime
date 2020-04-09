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
            Func<IFilterProcessor<TFilterDefinition>> createFilter,
            CancellationToken cancellationToken)
            where TResponse : IMessage
            where TRequest : IMessage
            where TFilterDefinition : IFilterDefinition
        {
            var targetStream = new StreamId { Value = eventProcessorId };
            try
            {
                ThrowIfIllegalTargetStream(targetStream);
                _logger.Debug($"Registering filter '{eventProcessorId}' in scope '{scope}'");

                await RegisterForAllTenants(scope, sourceStream, targetStream, createFilter, cancellationToken).ConfigureAwait(false);

                await dispatcher.HandleCalls().ConfigureAwait(false);
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
                UnregisterForAllTenants(scope, sourceStream, targetStream);
                _logger.Debug($"Filter client disconnected - '{eventProcessorId}'");
            }
        }

        async Task RegisterForAllTenants<TFilterDefinition>(
            ScopeId scope,
            StreamId sourceStream,
            StreamId targetStream,
            Func<IFilterProcessor<TFilterDefinition>> createFilter,
            CancellationToken cancellationToken)
            where TFilterDefinition : IFilterDefinition
        {
            _logger.Debug($"Registering filter with source stream '{sourceStream}' and target stream '{targetStream}' in scope '{scope}' for stream for {_tenants.All.Count} tenants");

            foreach (var tenant in _tenants.All)
            {
                try
                {
                    _executionContextManager.CurrentFor(tenant);
                    var filter = createFilter();
                    await _filterRegistryFactory().Register(filter, cancellationToken).ConfigureAwait(false);
                    _streamProcessorsFactory().Register(filter, _eventsFromStreamsFetcherFactory(), sourceStream);
                }
                catch (CouldNotRegisterFilter ex)
                {
                    _logger.Error(ex, $"The filter for stream '{targetStream}' in scope '{scope}' for tenant '{tenant}'. '{ex.Message}'");
                }
            }
        }

        void UnregisterForAllTenants(ScopeId scope, StreamId sourceStream, StreamId targetStream)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Unregistering filter with source stream '{sourceStream}' and target stream '{targetStream}' in scope '{scope}' for {tenants.Count} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _filterRegistryFactory().Unregister(targetStream);
                _streamProcessorsFactory().Unregister(scope, targetStream.Value, sourceStream);
            });
        }

        void ThrowIfIllegalTargetStream(StreamId stream)
        {
            if (stream.IsNonWriteable) throw new CannotFilterToNonWriteableStream(stream);
        }
    }
}