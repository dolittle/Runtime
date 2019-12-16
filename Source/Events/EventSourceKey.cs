// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Concepts;
using Dolittle.Events;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the key for an event source (the <see cref="EventSourceId">id </see> and <see cref="ArtifactId">artifact</see>).
    /// </summary>
    /// <remarks>
    /// An <see cref="EventSourceId" /> for an <see cref="IEventSource" /> is not unique.  Multiples event sources can share the same Id, given they are different artifact types (aggregates).
    /// </remarks>
    public class EventSourceKey : Value<EventSourceKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventSourceKey"/> class.
        /// </summary>
        /// <param name="id">The Event Source Id.</param>
        /// <param name="artifact">The Event Source Artifact.</param>
        public EventSourceKey(EventSourceId id, ArtifactId artifact)
        {
            Id = id;
            Artifact = artifact;
        }

        /// <summary>
        /// Gets the <see cref="EventSourceId" />.
        /// </summary>
        public EventSourceId Id { get; }

        /// <summary>
        /// Gets the <see cref="Artifact" />.
        /// </summary>
        public ArtifactId Artifact { get; }
    }
}
