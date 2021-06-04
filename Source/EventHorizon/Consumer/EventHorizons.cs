// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.EventHorizon.Consumer.Processing;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Microservices;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace Dolittle.Runtime.EventHorizon.Consumer
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizons" />.
    /// </summary>
    public class EventHorizons : IEventHorizons
    {
        readonly Services.Clients.IReverseCallClients _reverseCallClients;
        readonly ILoggerFactory _loggerFactory;

        public EventHorizons(Services.Clients.IReverseCallClients reverseCallClients, ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _reverseCallClients = reverseCallClients;
        }

        /// <inheritdoc/>
        public IEventHorizonProcessor Get(
            MicroserviceAddress connectionAddress,
            SubscriptionId subscription,
            AsyncProducerConsumerQueue<StreamEvent> eventsFromEventHorizon,
            CancellationToken cancellationToken)
            => new EventHorizonProcessor(
                _reverseCallClients.GetFor(
                    new EventHorizonProtocol(),
                    connectionAddress.Host,
                    connectionAddress.Port,
                    TimeSpan.FromSeconds(10)),
                subscription,
                eventsFromEventHorizon,
                _loggerFactory.CreateLogger<EventHorizonProcessor>(),
                cancellationToken);
    }
}
