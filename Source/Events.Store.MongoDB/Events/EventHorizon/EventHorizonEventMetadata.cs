// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents the metadata of an Event that has come through an event horizon from another microservice.
    /// </summary>
    public class EventHorizonEventMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHorizonEventMetadata"/> class.
        /// </summary>
        /// <param name="occurred">The date time offset of when the event occurred.</param>
        /// <param name="eventSource">The event source that applied the event.</param>
        /// <param name="correlation">The correlation.</param>
        /// <param name="microservice">The microservice that produced the event.</param>
        /// <param name="consumerTenant">The tenant that consumed the event.</param>
        /// <param name="producerTenant">The tenant that produced the event.</param>
        /// <param name="originEventLogSequenceNumber">The event log sequence number that the Event had in the microservice that it came from.</param>
        /// <param name="typeId">The type id of the event artifact.</param>
        /// <param name="typeGeneration">The generation of the event artifact.</param>
        public EventHorizonEventMetadata(DateTimeOffset occurred, Guid eventSource, Guid correlation, Guid microservice, Guid consumerTenant, Guid producerTenant, uint originEventLogSequenceNumber, Guid typeId, int typeGeneration)
        {
            Occurred = occurred;
            EventSource = eventSource;
            Correlation = correlation;
            Microservice = microservice;
            ConsumerTenant = consumerTenant;
            ProducerTenant = producerTenant;
            OriginEventLogSequenceNumber = originEventLogSequenceNumber;
            TypeId = typeId;
            TypeGeneration = typeGeneration;
        }

        /// <summary>
        /// Gets or sets the <see cref="DateTimeOffset"/> of when the event was committed to the event store.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public DateTimeOffset Occurred { get; set; }

        /// <summary>
        /// Gets or sets the event source id.
        /// </summary>
        public Guid EventSource { get; set; }

        /// <summary>
        /// Gets or sets the newly generated <see cref="CorrelationId"/> of the event.
        /// </summary>
        public Guid Correlation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Microservice"/> that produced the event.
        /// </summary>
        public Guid Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId"/> that the Event was consumed by.
        /// </summary>
        public Guid ConsumerTenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId"/> that the Event was produced by.
        /// </summary>
        public Guid ProducerTenant { get; set; }

        /// <summary>
        /// Gets or sets the event log sequence number that the Event had in the microservice that it came from.
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        public uint OriginEventLogSequenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"/> of the <see cref="Artifact"/> identitying the type of the event.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactGeneration"/> of the <see cref="Artifact"/> identifying the type of the event.
        /// </summary>
        public int TypeGeneration { get; set; }
    }
}