// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

extern alias contracts;

using System;
using System.Linq;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Tenancy;
using Dolittle.Services;
using Grpc.Core;
using static contracts::Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the implementation of <see creF="FiltersBase"/>.
    /// </summary>
    public class FiltersService : FiltersBase
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly FactoryFor<IStreamProcessors> _streamProcessorsFactory;
        readonly FactoryFor<IWriteEventsToStreams> _eventsToStreamsWriterFactory;
        readonly IReverseCallDispatchers _reverseCallDispatchers;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="FiltersService"/> class.
        /// </summary>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for current <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="tenants">The <see cref="ITenants"/> system.</param>
        /// <param name="streamProcessorsFactory"><see cref="FactoryFor{T}"/> the <see cref="IStreamProcessors"/> for registration management.</param>
        /// <param name="eventsToStreamsWriterFactory"><see cref="FactoryFor{T}"/> the <see cref="IWriteEventsToStreams">writer</see> for writing events.</param>
        /// <param name="reverseCallDispatchers">The <see cref="IReverseCallDispatchers"/> for working with reverse calls.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public FiltersService(
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
            IAsyncStreamReader<FilterClientToRuntimeResponse> runtimeStream,
            IServerStreamWriter<FilterRuntimeToClientRequest> clientStream,
            ServerCallContext context)
        {
            EventProcessorId eventProcessorId = Guid.Empty;
            StreamId streamId = Guid.Empty;
            try
            {
                var filterArguments = context.GetArgumentsMessage<FilterArguments>();

                eventProcessorId = filterArguments.FilterId.To<EventProcessorId>();
                streamId = filterArguments.StreamId.To<StreamId>();
                _logger.Debug($"Filter client connected - '{eventProcessorId}' - '{streamId}' - Method: {context.Method}");

                var dispatcher = _reverseCallDispatchers.GetDispatcherFor(
                    runtimeStream,
                    clientStream,
                    context,
                    _ => _.CallNumber,
                    _ => _.CallNumber);

                RegisterForAllTenants(dispatcher, eventProcessorId, streamId);

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
                UnregisterForAllTenants(eventProcessorId, streamId);
                _logger.Debug($"Filter client disconnected - '{eventProcessorId}'");
            }
        }

        void RegisterForAllTenants(
            IReverseCallDispatcher<FilterClientToRuntimeResponse, FilterRuntimeToClientRequest> callDispatcher,
            EventProcessorId eventProcessorId,
            StreamId streamId)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Registering filter '{eventProcessorId}' for stream '{streamId}' for {tenants.Count()} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                var filterProcessor = new FilterProcessor(
                    eventProcessorId,
                    callDispatcher,
                    streamId,
                    _eventsToStreamsWriterFactory(),
                    _executionContextManager,
                    _logger);

                _streamProcessorsFactory().Register(filterProcessor, streamId);
            });
        }

        void UnregisterForAllTenants(EventProcessorId eventProcessorId, StreamId streamId)
        {
            var tenants = _tenants.All;
            _logger.Debug($"Unregistering filter '{eventProcessorId}' for stream '{streamId}' for {tenants.Count()} tenants");
            tenants.ForEach(tenant =>
            {
                _executionContextManager.CurrentFor(tenant);
                _streamProcessorsFactory().Unregister(eventProcessorId, streamId);
            });
        }
    }
}