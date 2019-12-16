// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;

namespace Dolittle.Runtime.Events.Relativity
{
    /// <summary>
    /// Defines the single singularity in which is the destination for particles known as events.
    /// </summary>
    public interface ISingularity
    {
        /// <summary>
        /// Gets the <see cef="Application"/> the <see cref="ISingularity"/> represents.
        /// </summary>
        Application Application { get; }

        /// <summary>
        /// Gets the <see cref="BoundedContext"/> in which the <see cref="ISingularity"/> represents.
        /// </summary>
        BoundedContext BoundedContext { get; }

        /// <summary>
        /// Pass an <see cref="Processing.CommittedEventStreamWithContext"/> through.
        /// </summary>
        /// <param name="committedEventStreamWithContext"><see cref="Processing.CommittedEventStreamWithContext"/> to pass through.</param>
        /// <returns>True if it could pass through, false if not.</returns>
        /// <remarks>
        /// Not all singularities will be interested in events in a <see cref="Processing.CommittedEventStreamWithContext"/>
        /// If a singularity is not interested, it won't pass through and won't then do that.
        /// </remarks>
        bool PassThrough(Processing.CommittedEventStreamWithContext committedEventStreamWithContext);

        /// <summary>
        /// Determines wether or not the <see cref="ISingularity"/> is capable of receiving a <see cref="Processing.CommittedEventStreamWithContext"/>.
        /// </summary>
        /// <param name="committedEventStreamWithContext"><see cref="Processing.CommittedEventStreamWithContext"/> to ask for.</param>
        /// <returns>true if it can, false if not.</returns>
        bool CanPassThrough(Processing.CommittedEventStreamWithContext committedEventStreamWithContext);
    }
}