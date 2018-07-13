/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Collections.Generic;

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
        public Singularity(IEnumerable<ParticleSubscription> subscriptions)
        {
            Subscriptions = subscriptions;
        }

        /// <inheritdoc/>
        public IEnumerable<ParticleSubscription> Subscriptions {  get; }
    }
}