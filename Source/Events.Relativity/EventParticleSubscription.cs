/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Applications;

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
        public EventParticleSubscription(IEnumerable<ApplicationArtifactIdentifier> events)
        {
            Events = events;
        }

        /// <summary>
        /// Gets the events the subscription is for
        /// </summary>
        public IEnumerable<ApplicationArtifactIdentifier>   Events { get; }
    }
}