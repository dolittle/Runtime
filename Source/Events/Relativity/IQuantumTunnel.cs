/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Relativity
{

    /// <summary>
    /// Defines the tunnel in which particles known as events and their commits pass through
    /// Each singularity can connect to any event horizon, they establish a quantum tunnel for the purpose of moving particles across.
    /// </summary>
    public interface IQuantumTunnel
    {
        /// <summary>
        /// Event that gets fired when a <see cref="IQuantumTunnel"/> collapses
        /// </summary>
        event QuantumTunnelCollapsed    Collapsed;

        /// <summary>
        /// Pass an <see cref="CommittedEvents"/> through to the other side of the quantum tunnel
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEvents"/> to pass through</param>
        void PassThrough(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream);
    }
}