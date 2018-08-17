using System;
using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Events;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// A combination of the <see cref="EventId" />,  <see cref="EventMetadata" /> and a <see cref="PropertyBag" /> that is the persisted version of an <see cref="IEvent" />
    /// with the <see cref="CommitSequenceNumber" />
    /// </summary>
    public class CommittedEventEnvelope : Value<CommittedEventEnvelope>, IComparable<CommittedEventEnvelope>
    {
        /// <summary>
        /// Instantiates a new instance of an <see cref="CommittedEventEnvelope" />
        /// </summary>
        public CommittedEventEnvelope(CommitSequenceNumber commitSequence, EventId eventId, EventMetadata metadata, PropertyBag @event)
        {
            Metadata = metadata;
            Event = @event;
            Id = eventId;
            Version = new CommittedEventVersion(commitSequence,metadata.VersionedEventSource.Version.Commit,metadata.VersionedEventSource.Version.Sequence);
        }
        /// <summary>
        /// The <see cref="EventMetadata" /> associated with this persisted <see cref="IEvent" />
        /// </summary>
        /// <value></value>
        public EventMetadata Metadata { get; }
        /// <summary>
        /// A <see cref="PropertyBag" /> of the values associated with this <see cref="IEvent" />
        /// </summary>
        /// <value></value>
        public PropertyBag Event { get; }
        /// <summary>
        /// The <see cref="EventId" /> of this <see cref="IEvent" />
        /// </summary>
        /// <value></value>
        public EventId Id { get; }
        /// <summary>
        /// The <see cref="CommittedEventVersion" /> of this Event
        /// </summary>
        /// <value></value>
        public CommittedEventVersion Version { get; }

        /// <summary>
        /// Compares two <see cref="CommittedEventEnvelope" /> based on the <see cref="CommittedEventVersion" /> of each.
        /// </summary>
        /// <param name="other">The <see cref="CommittedEventEnvelope" /> to compare to</param>
        /// <returns>1 if greater, 0 if equal, -1 if less than</returns>
        public int CompareTo(CommittedEventEnvelope other)
        {
            if(other == null)
                return 1;

            if(this.Version == null && other.Version == null)
                return 0;

            if(this.Version == null && other.Version != null)
                return -1;

            return this.Version.CompareTo(other.Version);
        }
    }
}