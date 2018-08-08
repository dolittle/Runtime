/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Applications;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the single singularity in which is the destination for particles known as
    /// events
    /// </summary>
    public interface ISingularity
    {
        /// <summary>
        /// Gets the <see cef="Application"/> the <see cref="ISingularity"/> represents
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> in which the <see cref="ISingularity"/> represents
        /// </summary>
        BoundedContext BoundedContext { get; }

        /// <summary>
        /// Determines wether or not the <see cref="ISingularity"/> is capable of receiving a <see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="Dolittle.Runtime.Events.Store.CommittedEventStream"/> to ask for</param>
        /// <returns>True if it can, false if not</returns>
        bool CanReceive(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream);

        /// <summary>
        /// Gets the <see cref="IQuantumTunnel"/>
        /// </summary>
        IQuantumTunnel Tunnel { get; }
    }
}