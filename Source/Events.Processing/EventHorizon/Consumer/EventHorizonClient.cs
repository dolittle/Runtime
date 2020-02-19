// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
extern alias contracts;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using contracts::Dolittle.Runtime.Events.Processing;
using Dolittle.Applications;
using Dolittle.Collections;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Protobuf;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using Grpc.Core;
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
        readonly grpc.EventHorizon.EventHorizonClient _client;
        readonly IExecutionContextManager _executionContextManager;
        readonly FactoryFor<StreamProcessors> _getStreamProcessors;
        readonly FactoryFor<IReceivedEvents> _getReceivedEvents;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonClient"/> class.
        /// </summary>
        /// <param name="eventHorizons">The <see cref="EventHorizonsConfiguration" />.</param>
        /// <param name="client">The grpc client.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="getStreamProcessors">The <see cref="FactoryFor{StreamProcessors}" />.</param>
        /// <param name="getReceivedEvents">The <see cref="IReceivedEvents" />.</param>
        /// <param name="logger">The <see cref="ILogger" />.</param>
        public EventHorizonClient(
            EventHorizonsConfiguration eventHorizons,
            grpc.EventHorizon.EventHorizonClient client,
            IExecutionContextManager executionContextManager,
            FactoryFor<StreamProcessors> getStreamProcessors,
            FactoryFor<IReceivedEvents> getReceivedEvents,
            ILogger logger)
        {
            _eventHorizons = eventHorizons;
            _client = client;
            _executionContextManager = executionContextManager;
            _getStreamProcessors = getStreamProcessors;
            _getReceivedEvents = getReceivedEvents;
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
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            _executionContextManager.CurrentFor(subscriber);
            var publicEventsVersion = await _getReceivedEvents().GetVersionForTenant(microservice, producer, token).ConfigureAwait(false);

            var request = new EventHorizonSubscriberToPublisherRequest
            {
                Microservice = microservice.ToProtobuf(),
                ProducerTenant = producer.ToProtobuf(),
                SubscriberTenant = subscriber.ToProtobuf(),
                PublicEventsVersion = publicEventsVersion.Value
            };
            var call = _client.Subscribe(request, cancellationToken: token);
            var stream = call.ResponseStream;
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (!await stream.MoveNext().ConfigureAwait(false)) break;
                    _executionContextManager.CurrentFor(subscriber);
                    var @event = stream.Current.Event.ToCommittedEvent();
                    _logger.Information($"Tenant '{subscriber}' received public event '{@event.Type.Id}' from microservice '{microservice}' and tenant '{producer}'");
                    await _getReceivedEvents().Write(@event, microservice, producer, token).ConfigureAwait(false);
                }
                catch (RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.NotFound)
                {
                    await Task.Delay(1000).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    _logger.Error(e, $"Error while handling event from microservice '{microservice}' and tenant '{producer}'");
                    return;
                }
            }

            tokenSource.Dispose();
        }

        void SubscribeToMicroService(Microservice microservice, Subscriptions subscriptions) => subscriptions.ForEach(_ => SubscribeToTenantInMicroService(microservice, _.Key, _.Value));

        void SubscribeToTenantInMicroService(Microservice microservice, TenantId tenant, IEnumerable<TenantId> subscribers) => subscribers.ForEach(subscriber =>
        {
            var task = StartSubscription(microservice, tenant, subscriber);
        });
    }
}