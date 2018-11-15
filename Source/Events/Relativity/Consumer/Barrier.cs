/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/

using System.Collections.Generic;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Serialization.Protobuf;
using Dolittle.Runtime.Events.Relativity.Grpc;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Processing;
using Dolittle.DependencyInversion;
using Dolittle.Execution;
using Dolittle.Runtime.Tenancy;
using Dolittle.Lifecycle;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IBarrier"/>
    /// </summary>
    [SingletonPerTenant]
    public class Barrier : IBarrier
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

        /// <summary>
        /// Initializes a new instance of <see cref="Barrier"/>
        /// </summary>
        /// <param name="getGeodesics">A <see cref="FactoryFor{IGeodesics}"/> to get the correctly scoped geodesics for path offsetting</param>
        /// <param name="serializer"><see cref="ISerializer"/> used for serialization</param>
        /// <param name="getEventStore">A <see cref="FactoryFor{IEventStore}"/> to get the correctly scoped EventStore to persist incoming events to</param>
        /// <param name="eventProcessingHub"><see cref="IScopedEventProcessingHub"/> for processing incoming events</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        /// <param name="executionContextManager"><see cref="IExecutionContextManager"/> to set the correct context for processing events</param>
        /// <param name="tenants"><see cref="ITenants"/> all the tenants that we will process events for</param>
        /// <param name="tenantOffsetRepository"></param>
        /// <param name="application"></param>
        /// <param name="boundedContext"></param>
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

        /// <inheritdoc/>
        public void Penetrate(EventHorizonKey destinationKey, string url, IEnumerable<Artifact> events)
        {
            _logger.Information($"Penetrate barrier for quantum tunnel towards event horizon running at '{url}'");
            new QuantumTunnelConnection(
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