// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents an event stored in the MongoDB event store.
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The event log sequence number of this event.</param>
        /// <param name="metadata">The The event metadata.</param>
        /// <param name="aggregate">The aggregate metadata.</param>
        /// <param name="content">The event content.</param>
        public Event(uint eventLogSequenceNumber, EventMetadata metadata, AggregateMetadata aggregate, BsonDocument content)
        {
            EventLogSequenceNumber = eventLogSequenceNumber;
            Metadata = metadata;
            Aggregate = aggregate;
            Content = content;
        }

        /// <summary>
        /// Gets or sets the event log sequence number of the event.
        /// </summary>
        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public uint EventLogSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="EventMetadata"/> containing the platform generated event information.
        /// </summary>
        public EventMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the event sourcing specific <see cref="AggregateMetadata"/>.
        /// </summary>
        public AggregateMetadata Aggregate { get; set; }

        /// <summary>
        /// Gets or sets the domain specific event data.
        /// </summary>
        public BsonDocument Content { get; set; }
    }
}