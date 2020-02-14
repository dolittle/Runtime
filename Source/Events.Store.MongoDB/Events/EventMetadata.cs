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
    /// Represents the platform generated information about an event that is stored alongside the domain specific data in the event store.
    /// </summary>
    public class EventMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMetadata"/> class.
        /// </summary>
        /// <param name="occurred">The date time offset of when the event occurred.</param>
        /// <param name="eventSource">The event source that applied the event.</param>
        /// <param name="correlation">The correlation.</param>
        /// <param name="microservice">The microservice.</param>
        /// <param name="tenant">The tenant.</param>
        /// <param name="causeType">The type of the cause.</param>
        /// <param name="causePosition">The position of the cause.</param>
        /// <param name="typeId">The id of the event artifact type.</param>
        /// <param name="typeGeneration">The generation of the event artifact.</param>
        public EventMetadata(DateTimeOffset occurred, Guid eventSource, Guid correlation, Guid microservice, Guid tenant, CauseType causeType, uint causePosition, Guid typeId, int typeGeneration)
        {
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