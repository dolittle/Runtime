/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Serialization.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of the <see cref="QuantumTunnelService.QuantumTunnelServiceBase"/>
    /// </summary>
    public class QuantumTunnelServiceImplementation : QuantumTunnelService.QuantumTunnelServiceBase
    {
        readonly IEventHorizon _eventHorizon;
        readonly ISerializer _serializer;
        readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelServiceImplementation"/>
        /// </summary>
        /// <param name="eventHorizon"><see cref="IEventHorizon"/> to work with</param>
        /// <param name="serializer"><see cref="ISerializer"/> to be used for serialization</param>
        /// <param name="logger"></param>
        public QuantumTunnelServiceImplementation(
            IEventHorizon eventHorizon,
            ISerializer serializer,
            ILogger logger)
        {
            _eventHorizon = eventHorizon;
            _serializer = serializer;
            _logger = logger;
        }

        /// <inheritdoc/>
        public override async Task Open(OpenTunnelMessage request, IServerStreamWriter<CommittedEventStreamParticleMessage> responseStream, ServerCallContext context)
        {
            var tunnel = new QuantumTunnel(_serializer, responseStream);
            var application = (Application) new Guid(request.Application.ToByteArray());
            var boundedContext = (BoundedContext) new Guid(request.Application.ToByteArray());
            var events = request
                .Events
                .Select(@event =>
                    new Artifact(
                        new Guid(
                            @event.Event.ToByteArray()),
                            @event.Generation)
                )
                .ToArray();

            var subscription = new EventParticleSubscription(events);

            _logger.Information($"Opening up a quantum tunnel for bounded context '{boundedContext}' in application '{application}'");

            
            var singularity = new Singularity(application, boundedContext, tunnel, subscription);
            _eventHorizon.GravitateTowards(singularity);
            tunnel.Collapsed += qt => _eventHorizon.Collapse(singularity);

            await tunnel.Open();

            _logger.Information("Quantum tunnel collapsed for bounded context '{boundedContext}' in application '{application}'");
            

            // Create a quantum tunnel

            // Create subscriptions

            // Create location segment names from strings - proper type

            // Create a singularity from application and location with the quantum tunnel and subscriptions

            // Register the singularity with the event horizon

            // When disconnected :
            // - Set singularity in disconnected state
            // - timeout after a while and then remove singularity - collapse
            // - Disable the tunnel
            // - If a singularity comes back before singularity is collapsed - establish a new tunnel for the singularity

            await Task.CompletedTask;
        }
    }
}