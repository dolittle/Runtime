/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;
using Grpc.Core;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of the <see cref="QuantumTunnelService.QuantumTunnelServiceBase"/>
    /// </summary>
    public class QuantumTunnelServiceImplementation : QuantumTunnelService.QuantumTunnelServiceBase
    {
        /// <summary>
        /// Initializes a new instance of <see cref="QuantumTunnelServiceImplementation"/>
        /// </summary>
        /// <param name="eventHorizon"></param>
        public QuantumTunnelServiceImplementation(IEventHorizon eventHorizon)
        {

        }

        /// <inheritdoc/>
        public override async Task Open(OpenTunnelMessage request, IServerStreamWriter<EventParticleMessage> responseStream, ServerCallContext context)
        {
            await Task.CompletedTask;

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
        }
    }
}