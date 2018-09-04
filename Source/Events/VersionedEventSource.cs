using System;
using Dolittle.Concepts;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// A unique identifier of a particular EventSource (instance, type and version)
    /// </summary>
    public class VersionedEventSource : Value<VersionedEventSource>
    {
        /// <summary>
        /// Instantiates a new instance of a <see cref="VersionedEventSource" /> set to the initial version
        /// </summary>
        /// <param name="eventSource">The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" /></param>
        /// <param name="artifact">The <see cref="ArtifactId" /> that uniquely identifies the type of this event source</param>
        /// <returns></returns>
        public VersionedEventSource(EventSourceId eventSource, ArtifactId artifact): this(EventSourceVersion.Initial, eventSource, artifact) { }

        /// <summary>
        /// Instantiates a new instance of a <see cref="VersionedEventSource" /> set to the supplied version
        /// </summary>
        /// <param name="version">The <see cref="EventSourceVersion" /> of this instance</param>
        /// <param name="eventSource">The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" /></param>
        /// <param name="artifact">The <see cref="ArtifactId" /> that uniquely identifies the type of this event source</param>
        public VersionedEventSource(EventSourceVersion version, EventSourceId eventSource, ArtifactId artifact)
        {
            Version = version;
            EventSource = eventSource;
            Artifact = artifact;
        }
        /// <summary>
        /// The <see cref="EventSourceVersion" /> of this instance
        /// </summary>
        /// <value></value>
        public EventSourceVersion Version { get; }
        /// <summary>
        /// The <see cref="EventSourceId">Id</see> for this particular <see cref="IEventSource" />
        /// </summary>
        /// <value></value>
        public EventSourceId EventSource { get; }
        /// <summary>
        /// The <see cref="ArtifactId" /> that uniquely identifies the type of this event source
        /// </summary>
        /// <value></value>
        public ArtifactId Artifact { get; }

        /// <summary>
        /// Creates a <see cref="CommittedEventVersion" /> based upon this <see cref="VersionedEventSource" /> 
        /// </summary>
        /// <param name="commitSequence">the <see cref="CommitSequenceNumber" /></param>
        /// <returns>The <see cref="CommittedEventVersion" /> based upon this <see cref="VersionedEventSource" /> </returns>
        public CommittedEventVersion ToCommittedEventVersion(CommitSequenceNumber commitSequence)
        {
            return this.Version.ToCommittedEventVersion(commitSequence);
        }
    }
}
