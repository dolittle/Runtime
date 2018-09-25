/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Threading.Tasks;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Delegate representing a callback for when a <see cref="IQuantumTunnel"/> collapses
    /// </summary>
    /// <param name="tunnel"><see cref="IQuantumTunnel"/> that collapsed</param>
    public delegate void QuantumTunnelCollapsed(IQuantumTunnel tunnel);
}