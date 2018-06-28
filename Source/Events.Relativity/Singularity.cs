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
        /// 
        /// </summary>
        /// <param name="subscriptions"></param>
        public Singularity(IEnumerable<ParticleSubscription> subscriptions)
        {

        }

        /// <inheritdoc/>
        public IEnumerable<ParticleSubscription> Subscriptions {  get; }
    }
}