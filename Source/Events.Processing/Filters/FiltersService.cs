// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Streams;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IFilterRegistry> _filterRegistryFactory;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly FactoryFor<IFetchEventsFromStreams> _eventsFromStreamsFetcherFactory;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="filterRegistryFactory">The <see cref="FactoryFor{T}" /> the <see cref="IFilterRegistry" />.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="eventsFromStreamsFetcherFactory"><see cref="FactoryFor{T}"/> the <see cref="IFetchEventsFromStreams">fetcher</see> for fetching events.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            FactoryFor<IFilterRegistry> filterRegistryFactory,
            FactoryFor<IStreamProcessors> streamProcessorsFactory,
            FactoryFor<IWriteEventsToStreams> eventsToStreamsWriterFactory,
            FactoryFor<IFetchEventsFromStreams> eventsFromStreamsFetcherFactory,
            IReverseCallDispatchers reverseCallDispatchers,
            ILogger logger)
        {
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _filterRegistryFactory = filterRegistryFactory;
            _streamProcessorsFactory = streamProcessorsFactory;
            _eventsToStreamsWriterFactory = eventsToStreamsWriterFactory;
            _reverseCallDispatchers = reverseCallDispatchers;
            _logger = logger;
            _eventsFromStreamsFetcherFactory = eventsFromStreamsFetcherFactory;
        }

        /// <inheritdoc/>
        public override async Task Connect(
            IAsyncStreamReader<FilterClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            EventProcessorId eventProcessorId = Guid.Empty;
            var streamId = StreamId.AllStreamId;
            var scope = ScopeId.Default;
            try
            {
                var filterArguments = context.GetArgumentsMessage<FilterArguments>();
                eventProcessorId = filterArguments.Filter.To<EventProcessorId>();
                ThrowIfIllegalTargetStream(streamId);
                _logger.Debug($"Filter client connected - '{eventProcessorId}' - '{streamId}' - Method: {context.Method}");

                var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                    runtimeStream,
                    clientStream,
                    context,
                    _ => _.CallNumber,
                    _ => _.CallNumber);

                await RegisterForAllTenants(dispatcher, scope, eventProcessorId, streamId, context.CancellationToken).ConfigureAwait(false);

                await dispatcher.WaitTillDisconnected().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                if (!context.CancellationToken.IsCancellationRequested)
                {
                    _logger.Error(ex, $"Error occurred while handling filter client '{eventProcessorId}'");
                }
            }
            finally
            {
                UnregisterForAllTenants(scope, eventProcessorId, streamId);
                _logger.Debug($"Filter client disconnected - '{eventProcessorId}'");
            }
        }

        async Task RegisterForAllTenants(
            IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> callDispatcher,
            ScopeId scope,
            EventProcessorId eventProcessorId,
            StreamId streamId,
            CancellationToken cancellationToken)
        {
            _logger.Debug($"Registering filter '{eventProcessorId}' in scope '{scope}' for stream '{streamId}' for {_tenants.All.Count()} tenants");

            foreach (var tenant in _tenants.All)
            {
                try
                {
                    _executionContextManager.CurrentFor(tenant);
                    var filter = new FilterProcessor(
                        scope,
                        new RemoteFilterDefinition(streamId, eventProcessorId.Value),
                        callDispatcher,
                        _eventsToStreamsWriterFactory(),
                        _executionContextManager,
                        _logger);

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
            _logger.Debug($"Unregistering filter '{eventProcessorId}' in scope '{scope}' on stream '{streamId}' for {tenants.Count()} tenants");
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