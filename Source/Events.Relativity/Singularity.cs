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
        /// <param name="application"><see cref="ApplicationName">Application</see> representing the singularity</param>
        /// <param name="applicationLocation"><see cref="IEnumerable{IApplicationLocationSegmentName}"/> representing the location within the <see cref="Application"/> representing the singularity</param>
        public Singularity(
            IEnumerable<EventParticleSubscription> subscriptions,
            ApplicationName application,
            IEnumerable<IApplicationLocationSegmentName> applicationLocation)
        {
            Subscriptions = subscriptions;
            Application = application;
            ApplicationLocation = applicationLocation;
        }

        /// <inheritdoc/>
        public IEnumerable<EventParticleSubscription> Subscriptions {  get; }

        /// <inheritdoc/>
        public ApplicationName Application { get; }

        /// <inheritdoc/>
        public IEnumerable<IApplicationLocationSegmentName> ApplicationLocation { get; }
    }
}