/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;
using Dolittle.Applications;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Represents an implementation of <see cref="ISingularity"/>
    /// </summary>
    public class Singularity : ISingularity
    {
        /// <summary>
        /// Initializes a new instance of <see cref="Singularity"/>
        /// </summary>
        /// <param name="subscriptions"><see cref="IEnumerable{ParticleSubscription}">Subscriptions</see></param>
        /// <param name="application"><see cref="Application">Application</see> representing the singularity</param>
        /// <param name="boundedContext"><see cref="BoundedContext"/> representing the bounded context of the singularity</param>
        public Singularity(
            IEnumerable<EventParticleSubscription> subscriptions,
            Application application,
            BoundedContext boundedContext)
        {
            Subscriptions = subscriptions;
            Application = application;
            BoundedContext = boundedContext;
        }

        /// <inheritdoc/>
        public IEnumerable<EventParticleSubscription> Subscriptions {  get; }

        /// <inheritdoc/>
        public Application Application { get; }

        /// <inheritdoc/>
        public BoundedContext BoundedContext { get; }
    }
}