// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using Dolittle.Services.Clients;
using Dolittle.Tenancy;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    [Singleton]
    public class ConsumerClient : IConsumerClient
    {
        readonly IClientManager _clientManager;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<StreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IWriteReceivedEvents> _getReceivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="clientManager">The <see cref="IClientManager" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{IStreamProcessors}" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{IStreamProcessorStateRepository}" />.</param>
        /// <param name="getReceivedEventsWriter">The <see cref="FactoryFor{IWriteReceivedEvents}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            IClientManager clientManager,
            IExecutionContextManager executionContextManager,
            FactoryFor<StreamProcessors> getStreamProcessors,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IWriteReceivedEvents> getReceivedEventsWriter,
            ILogger logger)
        {
            _clientManager = clientManager;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getReceivedEventsWriter = getReceivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public async Task SubscribeTo(Microservice microservice, TenantId tenant, MicroserviceHost host, MicroservicePort port)
        {
            while (true)
            {
                var subscribingTenant = _executionContextManager.Current.Tenant;
                var streamProcessorId = new StreamProcessorId(tenant.Value, microservice.Value);
                _logger.Debug($"Tenant '{subscribingTenant}' is subscribing to events from tenant '{tenant} in microservice '{microservice}' on '{host}:{port}'");
                try
                {
#pragma warning disable CA2000
                    var tokenSource = new CancellationTokenSource();
                    _executionContextManager.CurrentFor(subscribingTenant);
                    var publicEventsPosition = (await _getStreamProcessorStates().GetOrAddNew(streamProcessorId).ConfigureAwait(false)).Position;
                    var eventsFetcher = new EventsFromEventHorizonFetcher(
                        _clientManager.Get<grpc.Consumer.ConsumerClient>(host, port).Subscribe(
                            new grpc.ConsumerSubscription
                            {
                                Tenant = tenant.ToProtobuf(),
                                PublicEventsPosition = publicEventsPosition.Value
                            },
                            cancellationToken: tokenSource.Token),
                        () =>
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                _logger.Debug($"Canceling cancellation token source for Event Horizon from tenant '{subscribingTenant}' to tenant '{tenant}' in microservice '{microservice}'");
                                tokenSource.Cancel();
                            }
                        },
                        _logger);
                    _getStreamProcessors().Register(
                        new ReceivedEventsProcessor(microservice, tenant, _getReceivedEventsWriter(), _logger),
                        eventsFetcher,
                        microservice.Value,
                        tokenSource);
#pragma warning restore CA2000

                    while (!tokenSource.IsCancellationRequested)
                    {
                        await Task.Delay(5000).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error occurred while handling Event Horizon to microservice '{microservice}' and tenant '{tenant}'");
                }
                finally
                {
                    _logger.Debug($"Disconnecting Event Horizon from tenant '{subscribingTenant}' to microservice '{microservice}' and tenant '{tenant}'");
                    _executionContextManager.CurrentFor(subscribingTenant);
                    _getStreamProcessors().Unregister(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId);
                }

                await Task.Delay(5000).ConfigureAwait(false);
            }
        }
    }
}