// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Events.Relativity.Microservice;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Runtime.Microservices;
using Dolittle.Serialization.Protobuf;
using Dolittle.Services;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the binder that gives us the <see cref="QuantumTunnelService"/>.
    /// </summary>
    /// <remarks>
    /// In order to observe black holes and its event horizons, one can do so through observing the gravitational lens.
    /// The service exposed enables the server to see these.
    /// </remarks>
    public class GravitationalLens : ICanBindMicroserviceServices
    {
        readonly IEventHorizon _eventHorizon;
        readonly ISerializer _serializer;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly IFetchUnprocessedCommits _fetchUnprocessedCommits;

        /// <summary>
        /// Initializes a new instance of the <see cref="GravitationalLens"/> class.
        /// </summary>
        /// <param name="eventHorizon">The <see cref="IEventHorizon"/>.</param>
        /// <param name="serializer"><see cref="ISerializer"/> for serializing payloads.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> for dealing with <see cref="Execution.ExecutionContext"/>.</param>
        /// <param name="fetchUnprocessedCommits">An <see cref="IFetchUnprocessedCommits" /> to fetch unprocessed commits.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging.</param>
        public GravitationalLens(
            IEventHorizon eventHorizon,
            ISerializer serializer,
            IExecutionContextManager executionContextManager,
            IFetchUnprocessedCommits fetchUnprocessedCommits,
            ILogger logger)
        {
            _serializer = serializer;
            _executionContextManager = executionContextManager;
            _logger = logger;
            _eventHorizon = eventHorizon;
            _fetchUnprocessedCommits = fetchUnprocessedCommits;
        }

        /// <inheritdoc/>
        public ServiceAspect Aspect => "Events";

        /// <inheritdoc/>
        public IEnumerable<Service> BindServices()
        {
            var service = new QuantumTunnelServiceImplementation(_eventHorizon, _serializer, _logger);
            return new Service[]
            {
                new Service(service, QuantumTunnelService.BindService(service), QuantumTunnelService.Descriptor)
            };
        }
    }
}