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
    /// Represents the Event metadata for <see cref="PublicEvent" />.
    /// </summary>
    public class PublicEventMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PublicEventMetadata"/> class.
        /// </summary>
        /// <param name="eventLogVersion">The event log version of this public event.</param>
        /// <param name="occurred">The date time offset of when the event occurred.</param>
        /// <param name="eventSource">The event source that applied the event.</param>
        /// <param name="correlation">The correlation.</param>
        /// <param name="microservice">The microservice.</param>
        /// <param name="tenant">The tenant.</param>
        /// <param name="causeType">The type of the cause.</param>
        /// <param name="causePosition">The position of the cause.</param>
        /// <param name="typeId">The id of the event artifact type.</param>
        /// <param name="typeGeneration">The generation of the event artifact.</param>
        public PublicEventMetadata(uint eventLogVersion, DateTimeOffset occurred, Guid eventSource, Guid correlation, Guid microservice, Guid tenant, CauseType causeType, uint causePosition, Guid typeId, int typeGeneration)
        {
            EventLogVersion = eventLogVersion;
            Occurred = occurred;
            EventSource = eventSource;
            Correlation = correlation;
            Microservice = microservice;
            Tenant = tenant;
            CauseType = causeType;
            CausePosition = causePosition;
            TypeId = typeId;
            TypeGeneration = typeGeneration;
        }

        /// <summary>
        /// Gets or sets the event log version of the event.
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        public uint EventLogVersion { get; set; }

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
        /// Gets or sets the <see cref="CorrelationId"/> of the event.
        /// </summary>
        public Guid Correlation { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Microservice"/> that produced the event.
        /// </summary>
        public Guid Microservice { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TenantId"/> that the event was produced in.
        /// </summary>
        public Guid Tenant { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CauseType"/> identifying the <see cref="Cause"/> of the event.
        /// </summary>
        [BsonRepresentation(BsonType.String)]
        public CauseType CauseType { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CauseLogPosition"/> identifying the <see cref="Cause"/> of the event.
        /// </summary>
        [BsonRepresentation(BsonType.Int64)]
        public uint CausePosition { get; set; }

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