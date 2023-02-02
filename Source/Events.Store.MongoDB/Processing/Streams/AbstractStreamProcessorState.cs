// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents the persisted version of <see cref="IStreamProcessorState"/>.
/// </summary>
/// <remarks>
/// The <see cref="StreamProcessorStateDiscriminatorConvention"/> is used to deserialise either a
/// <see cref="StreamProcessorState"/> or a <see cref="Partitioned.PartitionedStreamProcessorState"/> from a stream
/// processor state collection.
/// </remarks>
[BsonIgnoreExtraElements]
public abstract class AbstractStreamProcessorState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AbstractStreamProcessorState"/> class.
    /// </summary>
    /// <param name="eventProcessorId">The <see cref="EventProcessor" />.</param>
    /// <param name="sourceStreamId">The <see cref="SourceStream" />.</param>
    /// <param name="position">The position in the stream (how many processed events).</param>
    /// <param name="eventLogPosition">The position in the event log</param>
    /// <param name="lastSuccessfullyProcessed">The timestamp of when the Stream was last processed successfully.</param>
    protected AbstractStreamProcessorState(
        Guid eventProcessorId,
        Guid sourceStreamId,
        ulong position,
        ulong eventLogPosition,
        DateTime lastSuccessfullyProcessed)
    {
        EventProcessor = eventProcessorId;
        SourceStream = sourceStreamId;
        Position = position;
        EventLogPosition = eventLogPosition;
        LastSuccessfullyProcessed = lastSuccessfullyProcessed;
    }

    /// <summary>
    /// Gets or sets the event processor id.
    /// </summary>
    public Guid EventProcessor { get; set; }

    /// <summary>
    /// Gets or sets the source stream id.
    /// </summary>
    public Guid SourceStream { get; set; }

    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong Position { get; set; }
    
    /// <summary>
    /// Gets or sets the position.
    /// </summary>
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong EventLogPosition { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the StreamProcessor has processed the stream.
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime LastSuccessfullyProcessed { get; set; }

    /// <summary>
    /// Converts the state to it's runtime representation.
    /// </summary>
    /// <returns>The converted <see cref="IStreamProcessorState" />.</returns>
    public abstract IStreamProcessorState ToRuntimeRepresentation();
}
