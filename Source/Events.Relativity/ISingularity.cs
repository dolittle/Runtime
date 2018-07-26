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
        /// Gets the <see cef="ApplicationName"/> the <see cref="ISingularity"/> represents
        /// </summary>
        ApplicationName Application { get; }

        /// <summary>
        /// Gets the <see cref="ApplicationLocation"/> in form of <see cref="IApplicationLocationSegmentName">names</see> within the <see cref="Application"/> in which the <see cref="ISingularity"/> represents
        /// </summary>
        IEnumerable<IApplicationLocationSegmentName> ApplicationLocation { get; }

        /// <summary>
        /// Gets the <see cref="IEnumerable{ParticleSubscription}"/> 
        /// </summary>
        IEnumerable<EventParticleSubscription> Subscriptions { get; }
    }
}