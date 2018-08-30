/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the point of no return for events.
    /// The event horizon represents the final entry for committed events. 
    /// At this point they can only be seen by other singularities.
    /// </summary>
    public interface IEventHorizon
    {
        /// <summary>
        /// Pass events through the <see cref="IEventHorizon"/>
        /// </summary>
        /// <param name="committedEventStream"><see cref="CommittedEvents"/> to pass through</param>
        void PassThrough(Dolittle.Runtime.Events.Store.CommittedEventStream committedEventStream);

        /// <summary>
        /// Gravitate towards <see cref="ISingularity"/>
        /// </summary>
        /// <param name="singularity"><see cref="ISingularity"/> that will get gravitated towards</param>
        void GravitateTowards(ISingularity singularity);

        /// <summary>
        /// When a singularity collapses, this method is called to let the <see cref="IEventHorizon"/> know
        /// </summary>
        /// <param name="singularity"><see cref="ISingularity"/> that collapsed</param>
        void Collapse(ISingularity singularity);

        /// <summary>
        /// Get the collection of <see cref="ISingularity">singularities</see> in the system
        /// </summary>
        IEnumerable<ISingularity>  Singularities { get; }
    }
}