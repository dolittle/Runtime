// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Microservices;
using Dolittle.Services.Clients;
using grpc = contracts::Dolittle.Runtime.EventHorizon;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IConsumerClient" />.
    /// </summary>
    [Singleton]
    public class ConsumerClient : IConsumerClient
    {
        readonly IEventFromEventHorizonValidator _eventFromEventHorizonValidator;
        readonly IClientManager _clientManager;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<IStreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IWriteEventHorizonEvents> _getReceivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsumerClient"/> class.
        /// </summary>
        /// <param name="eventFromEventHorizonValidator">The <see cref="IEventFromEventHorizonValidator" />.</param>
        /// <param name="clientManager">The <see cref="IClientManager" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{IStreamProcessors}" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{IStreamProcessorStateRepository}" />.</param>
        /// <param name="getReceivedEventsWriter">The <see cref="FactoryFor{IWriteReceivedEvents}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public ConsumerClient(
            IEventFromEventHorizonValidator eventFromEventHorizonValidator,
            IClientManager clientManager,
            IExecutionContextManager executionContextManager,
            FactoryFor<IStreamProcessors> getStreamProcessors,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IWriteEventHorizonEvents> getReceivedEventsWriter,
            ILogger logger)
        {
            _eventFromEventHorizonValidator = eventFromEventHorizonValidator;
            _clientManager = clientManager;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getReceivedEventsWriter = getReceivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void AcknowledgeConsent(EventHorizon eventHorizon, MicroserviceAddress microserviceAddress, CancellationToken cancellationToken)
        {
            var ackResponse = _clientManager
                .Get<grpc.Consumer.ConsumerClient>(microserviceAddress.Host, microserviceAddress.Port)
                .AcknowledgeConsent(new grpc.AcknowledgeRequest { Microservice = eventHorizon.ProducerMicroservice.ToProtobuf(), Tenant = eventHorizon.ProducerTenant.ToProtobuf() });

            if (!ackResponse.Acknowledged) throw new NoConsentForEventHorizon(eventHorizon);
        }

        /// <inheritdoc/>
        public async Task SubscribeTo(EventHorizon eventHorizon, MicroserviceAddress microserviceAddress, CancellationToken cancellationToken)
        {
            while (true)
            {
                var streamProcessorId = new StreamProcessorId(eventHorizon.ProducerTenant.Value, eventHorizon.ProducerMicroservice.Value);
                _logger.Debug($"Tenant '{eventHorizon.ConsumerTenant}' is subscribing to events from tenant '{eventHorizon.ProducerTenant} in microservice '{eventHorizon.ProducerMicroservice}' on '{microserviceAddress.Host}:{microserviceAddress.Port}'");
                try
                {
#pragma warning disable CA2000
                    var publicEventsPosition = (await _getStreamProcessorStates().GetOrAddNew(streamProcessorId).ConfigureAwait(false)).Position;
                    var eventsFetcher = new EventsFromEventHorizonFetcher(
                        (@event) => _eventFromEventHorizonValidator.Validate(@event, eventHorizon.ConsumerMicroservice, eventHorizon.ConsumerTenant, eventHorizon.ProducerMicroservice, eventHorizon.ProducerTenant),
                        _clientManager.Get<grpc.Consumer.ConsumerClient>(microserviceAddress.Host, microserviceAddress.Port).Subscribe(
                            new grpc.ConsumerSubscription
                            {
                                Tenant = eventHorizon.ProducerTenant.ToProtobuf(),
                                PublicEventsPosition = publicEventsPosition.Value
                            },
                            cancellationToken: cancellationToken),
                        () =>
                        {
                            if (!cancellationToken.IsCancellationRequested)
                            {
                                _logger.Debug($"Closing Event Horizon between consumer microservice '{eventHorizon.ConsumerMicroservice}' and tenant '{eventHorizon.ConsumerTenant}' and producer microservice '{eventHorizon.ProducerMicroservice}' and tenant '{eventHorizon.ProducerTenant}'");
                            }
                        },
                        _logger);
                    _getStreamProcessors().Register(
                        new EventHorizonEventProcessor(eventHorizon, _getReceivedEventsWriter(), _logger),
                        eventsFetcher,
                        eventHorizon.ProducerMicroservice.Value,
                        cancellationToken);
#pragma warning restore CA2000

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        await Task.Delay(5000).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, $"Error occurred while handling Event Horizon to microservice '{eventHorizon.ProducerMicroservice}' and tenant '{eventHorizon.ProducerTenant}'");
                    throw;
                }
                finally
                {
                    _logger.Debug($"Disconnecting Event Horizon from tenant '{eventHorizon.ConsumerTenant}' to microservice '{eventHorizon.ProducerMicroservice}' and tenant '{eventHorizon.ProducerTenant}'");
                    _executionContextManager.CurrentFor(eventHorizon.ConsumerTenant);
                    _getStreamProcessors().Unregister(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId);
                }

                await Task.Delay(5000).ConfigureAwait(false);
            }
        }
    }
}