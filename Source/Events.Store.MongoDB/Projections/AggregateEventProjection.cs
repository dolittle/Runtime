// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Projections;

class AggregateEventProjection
{
    /// <summary>
    /// Gets or sets the event log sequence number of the event.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong EventLogSequenceNumber { get; set; }

    /// <summary>
    /// Gets or sets the execution context.
    /// </summary>
    public required ExecutionContext ExecutionContext { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="EventMetadata"/> containing additional event information.
    /// </summary>
    public required  EventMetadata Metadata { get; set; }

    /// <summary>
    /// Gets or sets the event sourcing specific <see cref="AggregateMetadata"/>.
    /// </summary>
    public required  AggregateMetadata Aggregate { get; set; }

    /// <summary>
    /// Gets or sets the domain specific event data.
    /// </summary>
    public required  BsonDocument Content { get; set; }
}
