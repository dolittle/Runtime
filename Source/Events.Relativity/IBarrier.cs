/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the interface for working with the barrier
    /// </summary>
    /// <remarks>
    /// Its purpose is to be able to establish <see cref="IQuantumTunnel"/> towards a <see cref="IEventHorizon"/>
    /// </remarks>
    public interface IBarrier
    {
        /// <summary>
        /// Penetrate to a specific <see cref="IEventHorizon"/> with a given Url for specific <see cref="Artifact">events</see>
        /// </summary>
        /// <param name="url">Url to an <see cref="IEventHorizon"/> to penentrate to</param>
        /// <param name="events">For <see cref="IEnumerable{Artifact}">events</see></param>
        void Penetrate(string url, IEnumerable<Artifact> events);
    }
}
