// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Represents event sourcing specific aggregate root and event source metadata.
    /// </summary>
    public class AggregateMetadata
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateMetadata"/> class.
        /// </summary>
        public AggregateMetadata()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateMetadata"/> class.
        /// </summary>
        /// <param name="wasAppliedByAggregate">Whether the event was applied by an aggregate.</param>
        /// <param name="typeId">The artifact type id of the aggregate root.</param>
        /// <param name="typeGeneration">The artifact generation of the aggregate root.</param>
        /// <param name="version">The version of the aggregate root.</param>
        public AggregateMetadata(bool wasAppliedByAggregate, Guid typeId, int typeGeneration, ulong version)
        {
            WasAppliedByAggregate = wasAppliedByAggregate;
            TypeId = typeId;
            TypeGeneration = typeGeneration;
            Version = version;
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not this event was applied by an aggregate root to an event source.
        /// </summary>
        public bool WasAppliedByAggregate { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactId"/> of the <see cref="Artifact"/> identitying the type of the aggregate root.
        /// </summary>
        public Guid TypeId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ArtifactGeneration"/> of the <see cref="Artifact"/> identifying the type of the aggregate root.
        /// </summary>
        public int TypeGeneration { get; set; }

        /// <summary>
        /// Gets or sets the aggregate root version.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Version { get; set; }
    }
}