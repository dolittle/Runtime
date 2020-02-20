// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Applications;
using Dolittle.Applications.Configuration;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using grpc = contracts::Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Processing.EventHorizon
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonClient" />.
    /// </summary>
    [Singleton]
    public class EventHorizonClient : IEventHorizonClient
    {
        readonly EventHorizonsConfiguration _eventHorizons;
        readonly BoundedContextConfiguration _boundedContextConfiguration;
        readonly grpc.EventHorizon.EventHorizonClient _client;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<StreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IStreamProcessorStateRepository> _getStreamProcessorStates;
        readonly FactoryFor<IWriteReceivedEvents> _getReceivedEventsWriter;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonClient"/> class.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="EventHorizonsConfiguration" />.</param>
        /// <param name="boundedContextConfiguration">The <see cref="BoundedContextConfiguration" />.</param>
        /// <param name="client">The grpc client.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{IStreamProcessors}" />.</param>
        /// <param name="getStreamProcessorStates">The <see cref="FactoryFor{IStreamProcessorStateRepository}" />.</param>
        /// <param name="getReceivedEventsWriter">The <see cref="FactoryFor{IWriteReceivedEvents}" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHorizonClient(
            EventHorizonsConfiguration eventHorizons,
            BoundedContextConfiguration boundedContextConfiguration,
            grpc.EventHorizon.EventHorizonClient client,
            IExecutionContextManager executionContextManager,
            FactoryFor<StreamProcessors> getStreamProcessors,
            FactoryFor<IStreamProcessorStateRepository> getStreamProcessorStates,
            FactoryFor<IWriteReceivedEvents> getReceivedEventsWriter,
            ILogger logger)
        {
            _eventHorizons = eventHorizons;
            _boundedContextConfiguration = boundedContextConfiguration;
            _client = client;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _getStreamProcessorStates = getStreamProcessorStates;
            _getReceivedEventsWriter = getReceivedEventsWriter;
            _logger = logger;
        }

        /// <inheritdoc/>
        public void Subscribe()
        {
            _eventHorizons.ForEach(_ => SubscribeToMicroService(_.Key, _.Value));
        }

        /// <inheritdoc/>
        public async Task StartSubscription(Microservice microservice, TenantId producer, TenantId subscriber)
        {
            while (true)
            {
                var streamProcessorId = new StreamProcessorId(producer.Value, microservice.Value);
                _logger.Debug($"Tenant '{subscriber}' is subscribing to events from tenant '{producer} in microservice '{microservice}'");
                try
                {
#pragma warning disable CA2000
                    var tokenSource = new CancellationTokenSource();
                    _executionContextManager.CurrentFor(subscriber);
                    var publicEventsVersion = (await _getStreamProcessorStates().GetOrAddNew(streamProcessorId).ConfigureAwait(false)).Position;
                    var eventsFetcher = new EventsFromEventHorizonFetcher(
                        _client.Subscribe(
                            new EventHorizonSubscriberToPublisherRequest
                            {
                                Microservice = _boundedContextConfiguration.BoundedContext.Value.ToProtobuf(),
                                ProducerTenant = producer.ToProtobuf(),
                                SubscriberTenant = subscriber.ToProtobuf(),
                                PublicEventsVersion = publicEventsVersion.Value
                            },
                            cancellationToken: tokenSource.Token),
                        () =>
                        {
                            if (!tokenSource.IsCancellationRequested)
                            {
                                _logger.Debug($"Canceling cancellation token source for Event Horizon from tenant '{subscriber}' to tenant '{producer}' in microservice '{microservice}'");
                                tokenSource.Cancel();
                            }
                        },
                        _logger);
                    _getStreamProcessors().Register(
                        new ReceivedEventsProcessor(microservice, producer, _getReceivedEventsWriter(), _logger),
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
                    _logger.Error(ex, $"Error occurred while handling Event Horizon to microservice '{microservice}' and tenant '{producer}'");
                }
                finally
                {
                    _logger.Debug($"Disconnecting Event Horizon from tenant '{subscriber}' to microservice '{microservice}' and tenant '{producer}'");
                    _executionContextManager.CurrentFor(subscriber);
                    _getStreamProcessors().Unregister(streamProcessorId.EventProcessorId, streamProcessorId.SourceStreamId);
                }

                await Task.Delay(5000).ConfigureAwait(false);
            }
        }

        void SubscribeToMicroService(Microservice microservice, Subscriptions subscriptions) => subscriptions.ForEach(_ => SubscribeToTenantInMicroService(microservice, _.Key, _.Value));

        void SubscribeToTenantInMicroService(Microservice microservice, TenantId tenant, IEnumerable<TenantId> subscribers) => subscribers.ForEach(subscriber =>
        {
            var task = StartSubscription(microservice, tenant, subscriber);
        });
    }
}