// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Lifecycle;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEstablishEventHorizonConnections" />.
    /// </summary>
    [SingletonPerTenant]
    public class EstablishEventHorizonConnections : IEstablishEventHorizonConnections
    {
        readonly IClientManager _clientManager;
        readonly IAsyncPolicyFor<ConsumerClient> _policy;
        readonly IStreamProcessorStateRepository _streamProcessorStates;
        readonly IEventHorizons _eventHorizons;
        readonly ILoggerFactory _loggerFactory;
        readonly ILogger _logger;

        public EstablishEventHorizonConnections(
            IClientManager clientManager,
            IAsyncPolicyFor<ConsumerClient> policy,
            IStreamProcessorStateRepository streamProcessorStates,
            IEventHorizons eventHorizons,
            ILoggerFactory loggerFactory)
        {
            _clientManager = clientManager;
            _policy = policy;
            _streamProcessorStates = streamProcessorStates;
            _eventHorizons = eventHorizons;
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<EstablishEventHorizonConnections>();
        }

        /// <inheritdoc/>
        public IEventHorizonConnection Establish(
            SubscriptionId subscription,
            MicroserviceAddress connectionAddress,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon)
        {
            _logger.LogDebug("Establishing EventHorizonConnection: {subscription} to address {connectionAddress}", subscription, connectionAddress);
            var connection = new EventHorizonConnection(
                subscription,
                connectionAddress,
                _clientManager.Get<Contracts.Consumer.ConsumerClient>(connectionAddress.Host, connectionAddress.Port),
                _policy,
                _streamProcessorStates,
                _eventHorizons,
                eventsFromEventHorizon,
                _loggerFactory.CreateLogger<EventHorizonConnection>());

            connection.StartResilientConnection();
            return connection;
        }
    }
}
