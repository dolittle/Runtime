// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Delegate representing a callback for when a <see cref="IQuantumTunnel"/> collapses.
    /// </summary>
    /// <param name="tunnel"><see cref="IQuantumTunnel"/> that collapsed.</param>
    public delegate void QuantumTunnelCollapsed(IQuantumTunnel tunnel);
}