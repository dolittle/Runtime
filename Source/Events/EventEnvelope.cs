// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.PropertyBags;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents a combination of the <see cref="EventMetadata" /> and a <see cref="PropertyBag" /> that is the persisted version of an <see cref="IEvent" />.
    /// </summary>
    public class EventEnvelope : Value<EventEnvelope>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventEnvelope"/> class.
        /// </summary>
        /// <param name="metadata"><see cref="EventMetadata"/> for the event.</param>
        /// <param name="event">The event in the form of a <see cref="PropertyBag"/>.</param>
        public EventEnvelope(EventMetadata metadata, PropertyBag @event)
        {
            Metadata = metadata;
            Event = @event;
        }

        /// <summary>
        /// Gets the <see cref="EventMetadata" /> associated with this persisted <see cref="IEvent" />.
        /// </summary>
        public EventMetadata Metadata { get; }

        /// <summary>
        /// Gets a <see cref="PropertyBag" /> of the values associated with this <see cref="IEvent" />.
        /// </summary>
        public PropertyBag Event { get; }

        /// <summary>
        /// Gets the <see cref="EventId" /> of this <see cref="IEvent" />.
        /// </summary>
        public EventId Id => Metadata.Id;

        /// <summary>
        /// Converts the <see cref="EventEnvelope"/> into the <see cref="CommittedEventEnvelope">Committed version</see> with the supplied <see cref="CommitSequenceNumber"/>.
        /// </summary>
        /// <param name="commitSequenceNumber"><see cref="CommitSequenceNumber"/> to create envelope for.</param>
        /// <returns>The new <see cref="CommittedEventEnvelope" />.</returns>
        public CommittedEventEnvelope ToCommittedEventEnvelope(CommitSequenceNumber commitSequenceNumber)
        {
            return new CommittedEventEnvelope(commitSequenceNumber, this.Metadata, this.Event);
        }
    }
}