// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the tunnel in which particles known as events and their commits pass through
    /// Each singularity can connect to any event horizon, they establish a quantum tunnel for the purpose of moving particles across.
    /// </summary>
    public interface IQuantumTunnel
    {
        /// <summary>
        /// Event that gets fired when a <see cref="IQuantumTunnel"/> collapses.
        /// </summary>
        event QuantumTunnelCollapsed Collapsed;

        /// <summary>
        /// Pass a <see cref="CommittedEventStreamWithContext"/> through to the other side of the quantum tunnel.
        /// </summary>
        /// <param name="contextualizedCommittedEventStream"><see cref="CommittedEventStreamWithContext"/> to pass through.</param>
        void PassThrough(CommittedEventStreamWithContext contextualizedCommittedEventStream);
    }
}