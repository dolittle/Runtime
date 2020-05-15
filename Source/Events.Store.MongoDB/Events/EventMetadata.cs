// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents the platform generated information about an event that is stored alongside the domain specific data in the event store.
    /// </summary>
    public class EventMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMetadata"/> class.
        /// </summary>
        /// <param name="occurred">The date time offset of when the event occurred.</param>
        /// <param name="eventSource">The event source that applied the event.</param>
        /// <param name="typeId">The id of the event artifact type.</param>
        /// <param name="typeGeneration">The generation of the event artifact.</param>
        /// <param name="isPublic">Whether the Event is public.</param>
        /// <param name="fromEventHorizon">Wether the Event is from an EventHorizon.</param>
        /// <param name="originEventLogSequenceNumber">The Event's original event log sequence number if it came from EventHorizon.</param>
        public EventMetadata(
            DateTime occurred,
            Guid eventSource,
            Guid typeId,
            uint typeGeneration,
            bool isPublic,
            bool fromEventHorizon,
            ulong originEventLogSequenceNumber)
        {
            Occurred = occurred;
            EventSource = eventSource;
            TypeId = typeId;
            TypeGeneration = typeGeneration;
            Public = isPublic;
            FromEventHorizon = fromEventHorizon;
            OriginEventLogSequenceNumber = originEventLogSequenceNumber;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTime"/> of when the event was committed to the event store.
        /// </summary>
        /// <remarks>
        /// BsonType.Document saves a UTC DateTime, ticks and an offset(in minutes) to the document. This way we can
        /// query for the DateTime from the database and it looks nicer than the string representation.
        /// https://github.com/mongodb/mongo-csharp-driver/blob/master/src/MongoDB.Bson/Serialization/Serializers/DateTimeOffsetSerializer.cs#L158 .
        /// </remarks>
        public DateTime Occurred { get; set; }

        /// <summary>
        /// Gets or sets the event source id.
        /// </summary>
        public Guid EventSource { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"/> of the <see cref="Artifact"/> identitying the type of the event.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactGeneration"/> of the <see cref="Artifact"/> identifying the type of the event.
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        public uint TypeGeneration { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a public Event.
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this event came from EventHorizon.
        /// </summary>
        public bool FromEventHorizon { get; set; }

        /// <summary>
        /// Gets or sets the origin event log sequence number of the event if it came from EventHorizon.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong OriginEventLogSequenceNumber { get; set; }
    }
}
