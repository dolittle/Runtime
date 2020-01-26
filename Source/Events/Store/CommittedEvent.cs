// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents an event that has been committed.
    /// </summary>
    public class CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvent"/> class.
        /// </summary>
        /// <param name="artifact">The <see cref="ArtifactId"/>.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset">timestamp</see> for when it occurred.</param>
        /// <param name="data">The payload as JSON.</param>
        public CommittedEvent(
            ArtifactId artifact,
            DateTimeOffset occurred,
            string data)
        {
            Artifact = artifact;
            Occurred = occurred;
            Data = data;
        }

        /// <summary>
        /// Gets the <see cref="ArtifactId"/> for the event.
        /// </summary>
        public ArtifactId Artifact { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset"/> for when the event occurred.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// Gets the data of the event - the payload - in the form of a JSON structure.
        /// </summary>
        public string Data { get; }
    }
}