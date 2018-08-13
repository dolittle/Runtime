/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents the gravitational lens - which is in our world the system that holds a server
    /// in which <see cref="ISingularity">singularities</see> are connecting using
    /// <see cref="IQuantumTunnel">quantum tunnels</see> to
    /// </summary>
    public interface IGravitationalLens
    {
        /// <summary>
        /// Start observing for connections
        /// </summary>
        /// <param name="eventHorizon"><see cref="IEventHorizon"/> to observe for</param>
        void ObserveFor(IEventHorizon eventHorizon);
    }
}