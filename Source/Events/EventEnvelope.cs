using Dolittle.Concepts;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events;
using Dolittle.Events;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// A combination of the <see cref="EventMetadata" /> and a <see cref="PropertyBag" /> that is the persisted version of an <see cref="IEvent" />
    /// </summary>
    public class EventEnvelope : Value<EventEnvelope>
    {
        /// <summary>
        /// Instantiates a new instance of an <see cref="EventEnvelope" />
        /// </summary>
        public EventEnvelope(EventMetadata metadata, PropertyBag @event)
        {
            Metadata = metadata;
            Event = @event;
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
        public EventId Id => Metadata.Id;

        /// <summary>
        /// Converts the <see cref="EventEnvelope"/> into the <see cref="CommittedEventEnvelope">Committed version</see> with the supplied <see cref="CommitSequenceNumber"/>
        /// </summary>
        /// <param name="commitSequenceNumber"></param>
        /// <returns>The <see cref="CommittedEventEnvelope" /></returns>
        public CommittedEventEnvelope ToCommittedEventEnvelope(CommitSequenceNumber commitSequenceNumber)
        {
            return new CommittedEventEnvelope(commitSequenceNumber,this.Metadata,this.Event);
        }
    }
}