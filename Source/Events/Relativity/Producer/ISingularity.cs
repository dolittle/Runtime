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
        /// Gets the <see cef="Application"/> the <see cref="ISingularity"/> represents
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> in which the <see cref="ISingularity"/> represents
        /// </summary>
        BoundedContext BoundedContext { get; }

        /// <summary>
        /// Pass an <see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/> through
        /// </summary>
        /// <param name="committedEventStreamWithContext"><see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/> to pass through</param>
        /// <returns>True if it could pass through, false if not</returns>
        /// <remarks>
        /// Not all singularities will be interested in events in a <see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/>
        /// If a singularity is not interested, it won't pass through and won't then do that
        /// </remarks>
        bool PassThrough(Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext committedEventStreamWithContext);

        /// <summary>
        /// Determines wether or not the <see cref="ISingularity"/> is capable of receiving a <see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/>
        /// </summary>
        /// <param name="committedEventStreamWithContext"><see cref="Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext"/> to ask for</param>
        /// <returns>True if it can, false if not</returns>
        bool CanPassThrough(Dolittle.Runtime.Events.Processing.CommittedEventStreamWithContext committedEventStreamWithContext);
    }
}