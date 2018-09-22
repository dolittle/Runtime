namespace Dolittle.Runtime.Events
{
    using Dolittle.Events;
    using Dolittle.Runtime.Events.Store;

    /// <summary>
    /// Represent an instance of an Event that has been committed to the Event Store
    /// </summary>
    public class CommittedEvent
    {
        /// <summary>
        /// Instantiates an instance of a <see cref="CommittedEvent" />
        /// </summary>
        /// <param name="version">The committed event version</param>
        /// <param name="metadata">Metadata describing the event</param>
        /// <param name="event">The <see cref="IEvent">event</see> instance</param>
        public CommittedEvent(CommittedEventVersion version, EventMetadata metadata, IEvent @event)
        {
            Version = version;
            Metadata = metadata;
            Event = @event;
        }

        /// <summary>
        /// The Version of this Event, including the CommitSequenceNumber, the Event Source Commit Version and the Event Sequence Number
        /// </summary>
        /// <value></value>
        public CommittedEventVersion Version { get; }

        /// <summary>
        /// Metadata describing the event
        /// </summary>
        /// <value></value>
        public EventMetadata Metadata { get; }

        /// <summary>
        /// The Id of the Event
        /// </summary>
        /// <value></value>
        public EventId Id => Metadata.Id;
        /// <summary>
        /// The instance of the <see cref="IEvent">event</see> that was committed
        /// </summary>
        /// <value></value>
        public IEvent Event { get; }
    }
}