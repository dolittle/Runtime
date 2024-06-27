// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq.Expressions;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Represents an event stored in a stream in the MongoDB event store.
/// </summary>
public class StreamEvent: IEvent<StreamEvent>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamEvent"/> class.
    /// </summary>
    /// <param name="streamPosition">The position in the stream.</param>
    /// <param name="partition">The partition id.</param>
    /// <param name="executionContext">The execution context.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <param name="aggregate">The aggregate metadata.</param>
    /// <param name="eventHorizonMetadata">The event horizon metadata.</param>
    /// <param name="content">The event content.</param>
    public StreamEvent(
        ulong streamPosition,
        PartitionId partition,
        ExecutionContext executionContext,
        StreamEventMetadata metadata,
        AggregateMetadata? aggregate,
        EventHorizonMetadata? eventHorizonMetadata,
        BsonDocument content)
    {
        StreamPosition = streamPosition;
        Partition = partition;
        ExecutionContext = executionContext;
        Metadata = metadata;
        Aggregate = aggregate;
        EventHorizon = eventHorizonMetadata;
        Content = content;
    }

    /// <summary>
    /// Gets or sets stream position.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong StreamPosition { get; set; }

    public static Expression<Func<StreamEvent, object>> StreamPositionExpression { get; } = e => e.StreamPosition;

    [BsonIgnore] public ulong EventLogSequenceNumber => Metadata.EventLogSequenceNumber;

    /// <summary>
    /// Gets or sets the partition id.
    /// </summary>
    public string Partition { get; set; }

    /// <summary>
    /// Gets or sets the execution context.
    /// </summary>
    public ExecutionContext ExecutionContext { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="StreamEventMetadata"/> containing the platform generated event information.
    /// </summary>
    public StreamEventMetadata Metadata { get; set; }

    /// <summary>
    /// Gets or sets the event sourcing specific <see cref="AggregateMetadata"/>.
    /// </summary>
    [BsonIgnoreIfNull]
    public AggregateMetadata? Aggregate { get; set; }

    [BsonIgnore] public bool WasAppliedByAggregate => Aggregate is { WasAppliedByAggregate: true };
    
    /// <summary>
    /// Gets or sets the <see cref="EventHorizonMetadata" />.
    /// </summary>
    [BsonIgnoreIfNull]
    public EventHorizonMetadata? EventHorizon { get; set; }

    [BsonIgnore] public bool IsFromEventHorizon => EventHorizon is { FromEventHorizon: true };
    
    /// <summary>
    /// Gets or sets the domain specific event data.
    /// </summary>
    public BsonDocument Content { get; set; }
    
}
