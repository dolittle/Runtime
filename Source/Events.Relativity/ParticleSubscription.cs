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
    public class ParticleSubscription
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ParticleSubscription"/>
        /// </summary>
        /// <param name="events"></param>
        public ParticleSubscription(IEnumerable<ApplicationArtifactIdentifier> events)
        {
            Events = events;
        }

        /// <summary>
        /// Gets the events the subscription is for
        /// </summary>
        public IEnumerable<ApplicationArtifactIdentifier>   Events { get; }
        
    }
}