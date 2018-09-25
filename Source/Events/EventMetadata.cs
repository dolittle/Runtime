namespace Dolittle.Runtime.Events
{
    using System;
    using Dolittle.Concepts;
    using Dolittle.Runtime.Events;
    using Dolittle.Events;
    using Dolittle.Artifacts;
    using Dolittle.Execution;

    /// <summary>
    /// Represents the metadata associated with a particular <see cref="IEvent" /> instance>
    /// Exhibits value equality semantics
    /// </summary>
    public class EventMetadata : Value<EventMetadata>
    {
        /// <summary>
        /// Instantiates an instance of <see cref="EventMetadata" />
        /// </summary>
        /// <param name="eventId">An <see cref="EventId"/> that uniquely identifies this event</param>
        /// <param name="versionedEventSource">The <see cref="VersionedEventSource" /> that this event applies to.</param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system</param>
        /// <param name="artifact">The <see cref="Artifact" /> that represents this event</param>
        /// <param name="occurred">A timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.</param>
        /// <param name="originalContext">The <see cref="OriginalContext" /> of the <see cref="IEvent" /></param>
        public EventMetadata(EventId eventId, VersionedEventSource versionedEventSource, CorrelationId correlationId, Artifact artifact, DateTimeOffset occurred, OriginalContext originalContext)
        {
            Id = eventId;
            VersionedEventSource = versionedEventSource;
            CorrelationId = correlationId;
            Artifact = artifact;
            Occurred = occurred;
            OriginalContext = originalContext;
        }
        /// <summary>
        /// The <see cref="EventId" /> that uniquely identifies this Event
        /// </summary>
        /// <value></value>
        public EventId Id { get; }
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
        /// The <see cref="Artifact" /> that the event represents
        /// </summary>
        /// <value></value>
        public Artifact Artifact { get; }
        /// <summary>
        /// The <see cref="EventSourceId" /> identifying the <see cref="IEventSource" /> that this event applies to.
        /// </summary>
        public EventSourceId EventSourceId => VersionedEventSource != null ? VersionedEventSource.EventSource : EventSourceId.Empty;
        /// <summary>
        /// The <see cref="OriginalContext" /> derived from the <see cref="ExecutionContext" /> when this <see cref="IEvent" /> occurred
        /// </summary>
        /// <value></value>
        public OriginalContext OriginalContext { get; }
        /// <summary>
        /// A timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.
        /// </summary>
        /// <value></value>
        public DateTimeOffset Occurred { get; }
    }
}