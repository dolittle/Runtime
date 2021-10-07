// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Dolittle.Runtime.Events.Store.MongoDB.Migrations.ToV7.Models.Aggregates
{
    /// <summary>
    /// Represents the state of an Aggregate Root.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class AggregateRoot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateRoot"/> class.
        /// </summary>
        /// <param name="eventSource">The event source id.</param>
        /// <param name="aggregateType">The type of the aggregate root.</param>
        /// <param name="version">The version of the aggregate root.</param>
        public AggregateRoot(Guid eventSource, Guid aggregateType, ulong version)
        {
            EventSource = eventSource;
            AggregateType = aggregateType;
            Version = version;
        }

        /// <summary>
        /// Gets or sets the id of the Event Source.
        /// </summary>
        public Guid EventSource { get; set; }

        /// <summary>
        /// Gets or sets the type of the Aggregate Root.
        /// </summary>
        public Guid AggregateType { get; set; }

        /// <summary>
        /// Gets or sets the version of the Aggregate Root.
        /// </summary>
        [BsonRepresentation(BsonType.Decimal128)]
        public ulong Version { get; set; }
    }
}