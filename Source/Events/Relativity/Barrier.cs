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

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="IBarrier"/>
    /// </summary>
    public class Barrier : IBarrier
    {
        readonly ILogger _logger;
        readonly Application _application;
        readonly BoundedContext _boundedContext;
        readonly IGeodesics _geodesics;
        readonly ISerializer _serializer;
        readonly IEventStore _eventStore;
        private readonly IEventProcessors _eventProcessors;

        /// <summary>
        /// Initializes a new instance of <see cref="Barrier"/>
        /// </summary>
        /// <param name="application">The current <see cref="Application"/></param>
        /// <param name="boundedContext">The current <see cref="BoundedContext"/></param>
        /// <param name="geodesics">The <see cref="IGeodesics"/> for path offsetting</param>
        /// <param name="serializer"><see cref="ISerializer"/> used for serialization</param>
        /// <param name="eventStore"><see cref="IEventStore"/> to persist incoming events to</param>
        /// <param name="eventProcessors"><see cref="IEventProcessors"/> for processing incoming events</param>
        /// <param name="logger"><see cref="ILogger"/> for logging purposes</param>
        public Barrier(
            Application application,
            BoundedContext boundedContext,
            IGeodesics geodesics,
            ISerializer serializer,
            IEventStore eventStore,
            IEventProcessors eventProcessors,
            ILogger logger)
        {
            _logger = logger;
            _application = application;
            _boundedContext = boundedContext;
            _geodesics = geodesics;
            _serializer = serializer;
            _eventStore = eventStore;
            _eventProcessors = eventProcessors;
        }

        /// <inheritdoc/>
        public void Penetrate(Application destinationApplication, BoundedContext destinationBoundedContext, string url, IEnumerable<Artifact> events)
        {
            _logger.Information($"Penetrate barrier for quantum tunnel towards event horizon running at '{url}'");
            new QuantumTunnelConnection(
                _application,
                _boundedContext,
                destinationApplication,
                destinationBoundedContext,
                url,
                events,
                _geodesics,
                _eventStore,
                _eventProcessors,
                _serializer,
                _logger);
        }
    }

}