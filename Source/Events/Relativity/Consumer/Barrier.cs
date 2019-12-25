// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Lifecycle;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Relativity.Grpc;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Tenancy;
using Dolittle.Serialization.Protobuf;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IBarrier"/>.
    /// </summary>
    [SingletonPerTenant]
    public class Barrier : IBarrier, IDisposable
    {
        readonly ILogger _logger;
        readonly EventHorizonKey _key;
        readonly FactoryFor<IGeodesics> _getGeodesics;
        readonly ISerializer _serializer;
        readonly FactoryFor<IEventStore> _getEventStore;
        readonly IScopedEventProcessingHub _eventProcessingHub;
        readonly IExecutionContextManager _executionContextManager;
        readonly ITenants _tenants;
        readonly ITenantOffsetRepository _tenantOffsetRepository;

        readonly ConcurrentDictionary<EventHorizonKey, QuantumTunnelConnection> _connectionsPerEventHorizon = new ConcurrentDictionary<EventHorizonKey, QuantumTunnelConnection>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Barrier"/> class.
        /// </summary>
        /// <param name="getGeodesics">A <see cref="FactoryFor{IGeodesics}"/> to get the correctly scoped geodesics for path offsetting.</param>
        /// <param name="serializer"><see cref="ISerializer"/> used for serialization.</param>
        /// <param name="getEventStore">A <see cref="FactoryFor{IEventStore}"/> to get the correctly scoped EventStore to persist incoming events to.</param>
        /// <param name="eventProcessingHub"><see cref="IScopedEventProcessingHub"/> for processing incoming events.</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes.</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> to set the correct context for processing events.</param>
        /// <param name="tenants"><see cref="ITenants"/> all the tenants that we will process events for.</param>
        /// <param name="tenantOffsetRepository"><see cref="ITenantOffsetRepository"/> for working with the offsets per tenant.</param>
        /// <param name="application"><see cref="Application"/> running.</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> running.</param>
        public Barrier(
            FactoryFor<IGeodesics> getGeodesics,
            ISerializer serializer,
            FactoryFor<IEventStore> getEventStore,
            IScopedEventProcessingHub eventProcessingHub,
            ILogger logger,
            IExecutionContextManager executionContextManager,
            ITenants tenants,
            ITenantOffsetRepository tenantOffsetRepository,
            Application application,
            BoundedContext boundedContext)
        {
            _logger = logger;
            _getGeodesics = getGeodesics;
            _serializer = serializer;
            _getEventStore = getEventStore;
            _eventProcessingHub = eventProcessingHub;
            _executionContextManager = executionContextManager;
            _tenants = tenants;
            _tenantOffsetRepository = tenantOffsetRepository;

            _key = new EventHorizonKey(application, boundedContext);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="Barrier"/> class.
        /// </summary>
        ~Barrier()
        {
            Dispose();
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            foreach ((var _, var connection) in _connectionsPerEventHorizon)
            {
                connection.Dispose();
            }

            _connectionsPerEventHorizon.Clear();
        }

        /// <inheritdoc/>
        public void Penetrate(EventHorizonKey destinationKey, string url, IEnumerable<Artifact> events)
        {
            _logger.Information($"Penetrate barrier for quantum tunnel towards event horizon running at '{url}'");

            if (_connectionsPerEventHorizon.ContainsKey(destinationKey)) _connectionsPerEventHorizon[destinationKey].Dispose();

            _connectionsPerEventHorizon[destinationKey] = new QuantumTunnelConnection(
                _key,
                destinationKey,
                url,
                events,
                _getGeodesics,
                _getEventStore,
                _eventProcessingHub,
                _serializer,
                _logger,
                _executionContextManager,
                _tenants,
                _tenantOffsetRepository);
        }
    }
}