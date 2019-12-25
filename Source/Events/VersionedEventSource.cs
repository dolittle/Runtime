// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// A unique identifier of a particular EventSource (instance, type and version).
    /// </summary>
    public class VersionedEventSource : Value<VersionedEventSource>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedEventSource"/> class.
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" />.</param>
        /// <param name="artifact">The <see cref="ArtifactId" /> that uniquely identifies the type of this event source.</param>
        public VersionedEventSource(EventSourceId eventSource, ArtifactId artifact)
            : this(EventSourceVersion.Initial, new EventSourceKey(eventSource, artifact))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedEventSource"/> class.
        /// </summary>
        /// <param name="version">The <see cref="EventSourceVersion" /> of this instance.</param>
        /// <param name="key">The <see cref="EventSourceKey">Key</see> for this particular <see cref="IEventSource" />.</param>
        public VersionedEventSource(EventSourceVersion version, EventSourceKey key)
        {
            Version = version;
            Key = key;
        }

        /// <summary>
        /// Gets the <see cref="EventSourceVersion" /> of this instance.
        /// </summary>
        public EventSourceVersion Version { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" />.
        /// </summary>
        public EventSourceId EventSource => Key.Id;

        /// <summary>
        /// Gets the <see cref="ArtifactId" /> that uniquely identifies the type of this event source.
        /// </summary>
        public ArtifactId Artifact => Key.Artifact;

        /// <summary>
        /// Gets the <see cref="EventSourceKey" /> that uniquely identifies this event source.
        /// </summary>
        public EventSourceKey Key { get; }

        /// <summary>
        /// Creates a <see cref="CommittedEventVersion" /> based upon this <see cref="VersionedEventSource" />.
        /// </summary>
        /// <param name="commitSequence">the <see cref="CommitSequenceNumber" />.</param>
        /// <returns>The <see cref="CommittedEventVersion" /> based upon this <see cref="VersionedEventSource" />.</returns>
        public CommittedEventVersion ToCommittedEventVersion(CommitSequenceNumber commitSequence)
        {
            return Version.ToCommittedEventVersion(commitSequence);
        }
    }
}
