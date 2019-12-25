// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents a subscription for particles.
    /// </summary>
    public class EventParticleSubscription
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventParticleSubscription"/> class.
        /// </summary>
        /// <param name="events">Events in the form of <see cref="Artifact"/>.</param>
        public EventParticleSubscription(IEnumerable<Artifact> events)
        {
            Events = events;
        }

        /// <summary>
        /// Gets the events the subscription is for.
        /// </summary>
        public IEnumerable<Artifact> Events { get; }
    }
}