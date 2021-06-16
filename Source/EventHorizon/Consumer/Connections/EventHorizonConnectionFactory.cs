// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Microservices;
using Dolittle.Runtime.Services.Clients;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.EventHorizon.Consumer.Connections
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventHorizonConnectionFactory"/>.
    /// </summary>
    public class EventHorizonConnectionFactory : IEventHorizonConnectionFactory
    {
        readonly IReverseCallClients _reverseCallClients;
        readonly IMetricsCollector _metrics;
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonConnectionFactory"/> class.
        /// </summary>
        /// <param name="reverseCallClients">The reverse call clients to use for creating event horizon clients.</param>
        /// <param name="metrics">The system for collecting metrics.</param>
        /// <param name="loggerFactory">The logger factory to use for creating loggers for the event horizon connections</param>
        public EventHorizonConnectionFactory(IReverseCallClients reverseCallClients, IMetricsCollector metrics, ILoggerFactory loggerFactory)
        {
            _reverseCallClients = reverseCallClients;
            _metrics = metrics;
            _loggerFactory = loggerFactory;
        }


        /// <inheritdoc/>
        public IEventHorizonConnection Create(MicroserviceAddress connectionAddress)
        {
            var client = _reverseCallClients.GetFor(
                new EventHorizonProtocol(),
                connectionAddress.Host,
                connectionAddress.Port,
                TimeSpan.FromSeconds(10));

            return new EventHorizonConnection(
                client,
                _metrics,
                _loggerFactory.CreateLogger<EventHorizonConnection>());
        }
    }
}
