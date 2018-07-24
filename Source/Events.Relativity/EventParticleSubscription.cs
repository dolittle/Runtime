/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents a subscription for particles
    /// </summary>
    public class EventParticleSubscription
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventParticleSubscription"/>
        /// </summary>
        /// <param name="events"></param>
        public EventParticleSubscription(IEnumerable<Artifact> events)
        {
            Events = events;
        }

        /// <summary>
        /// Gets the events the subscription is for
        /// </summary>
        public IEnumerable<Artifact>   Events { get; }
    }
}