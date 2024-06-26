// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Represents an event stored in the MongoDB event store.
/// </summary>
public class Event: IEvent<Event>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Event"/> class.
    /// </summary>
    /// <param name="eventLogSequenceNumber">The event log sequence number of this event.</param>
    /// <param name="executionContext">The execution context.</param>
    /// <param name="metadata">The event metadata.</param>
    /// <param name="aggregate">The aggregate metadata.</param>
    /// <param name="eventHorizonMetadata">The event horizon metadata.</param>
    /// <param name="content">The event content.</param>
    public Event(
        ulong eventLogSequenceNumber,
        ExecutionContext executionContext,
        EventMetadata metadata,
        AggregateMetadata? aggregate,
        EventHorizonMetadata? eventHorizonMetadata,
        BsonDocument content)
    {
        EventLogSequenceNumber = eventLogSequenceNumber;
        ExecutionContext = executionContext;
        Metadata = metadata;
        Aggregate = aggregate;
        EventHorizon = eventHorizonMetadata;
        Content = content;
    }

    /// <summary>
    /// Gets or sets the event log sequence number of the event.
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong EventLogSequenceNumber { get; set; }

    // Do not serialize
    [BsonIgnore] public ulong StreamPosition => EventLogSequenceNumber;
    
    public static Expression<Func<Event, object>> StreamPositionExpression { get; } = e => e.EventLogSequenceNumber;

    /// <summary>
    /// Gets or sets the execution context.
    /// </summary>
    public ExecutionContext ExecutionContext { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="EventMetadata"/> containing additional event information.
    /// </summary>
    public EventMetadata Metadata { get; set; }

    /// <summary>
    /// Gets or sets the event sourcing specific <see cref="AggregateMetadata"/>.
    /// </summary>
    [BsonIgnoreIfNull]
    [MemberNotNullWhen(true, nameof(WasAppliedByAggregate))]
    public AggregateMetadata? Aggregate { get; set; }

    [BsonIgnore] public bool WasAppliedByAggregate => Aggregate is { WasAppliedByAggregate: true };

    /// <summary>
    /// Gets or sets the <see cref="EventHorizonMetadata" />.
    /// </summary>
    [BsonIgnoreIfNull]
    [MemberNotNullWhen(true, nameof(IsFromEventHorizon))]

    public EventHorizonMetadata? EventHorizon { get; set; }

    [BsonIgnore] public bool IsFromEventHorizon => EventHorizon is { FromEventHorizon: true };

    /// <summary>
    /// Gets or sets the domain specific event data.
    /// </summary>
    public BsonDocument Content { get; set; }
}
