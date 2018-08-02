namespace Dolittle.Runtime.Events.Store
{
    using System;
    using Dolittle.Concepts;
    using Dolittle.Runtime.Events;
    using Dolittle.Events;
    using Dolittle.Artifacts;

    /// <summary>
    /// Represents the metadata associated with a particular <see cref="IEvent" /> instance>
    /// Exhibits value equality semantics
    /// </summary>
    public class EventMetadata : Value<EventMetadata>
    {
        /// <summary>
        /// Instantiates an instance of <see cref="EventMetadata" />
        /// </summary>
        /// <param name="versionedEventSource">The <see cref="VersionedEventSource" /> that this event applies to.</param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system</param>
        /// <param name="generation">The <see cref="ArtifactGeneration" /> that the event represents</param>
        /// <param name="causedBy">The <see cref="CausedBy" /> instance that caused this <see cref="IEvent" /></param>
        /// <param name="occurred">A timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.</param>
        public EventMetadata(VersionedEventSource versionedEventSource, CorrelationId correlationId, ArtifactGeneration generation, CausedBy causedBy, DateTimeOffset occurred)
        {
            VersionedEventSource = versionedEventSource;
            CorrelationId = correlationId;
            ArtifactGeneration = generation;
            CausedBy = causedBy;
            Occurred = occurred;
        }
        /// <summary>
        /// The <see cref="VersionedEventSource" /> that this event applies to.
        /// </summary>
        /// <value></value>
        public VersionedEventSource VersionedEventSource { get; }
        /// <summary>
        /// A <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system
        /// </summary>
        /// <value></value>
        public CorrelationId CorrelationId { get; }
        /// <summary>
        /// The <see cref="ArtifactGeneration" /> that the event represents
        /// </summary>
        /// <value></value>
        public ArtifactGeneration ArtifactGeneration { get; }
        /// <summary>
        /// The <see cref="EventSourceId" /> identifying the <see cref="IEventSource" /> that this event applies to.
        /// </summary>
        public EventSourceId EventSourceId => VersionedEventSource != null ? VersionedEventSource.EventSource : EventSourceId.Empty;
        /// <summary>
        /// The <see cref="CausedBy" /> instance that caused this <see cref="IEvent" />
        /// </summary>
        /// <value></value>
        public CausedBy CausedBy { get; }
        /// <summary>
        /// A timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.
        /// </summary>
        /// <value></value>
        public DateTimeOffset Occurred { get; }
    }
}