// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Represents the Event metadata for a <see cref="StreamEvent" />.
/// </summary>
public class StreamEventMetadata : EventMetadata
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StreamEventMetadata"/> class.
    /// </summary>
    /// <param name="eventLogSequenceNumber">The event log sequence number.</param>
    /// <param name="occurred">The date time offset of when the event occurred.</param>
    /// <param name="eventSource">The event source that applied the event.</param>
    /// <param name="typeId">The id of the event artifact type.</param>
    /// <param name="typeGeneration">The generation of the event artifact.</param>
    /// <param name="isPublic">Whether the Event is public.</param>
    public StreamEventMetadata(
        ulong eventLogSequenceNumber,
        DateTime occurred,
        string eventSource,
        Guid typeId,
        uint typeGeneration,
        bool isPublic)
        : base(occurred, eventSource, typeId, typeGeneration, isPublic)
    {
        EventLogSequenceNumber = eventLogSequenceNumber;
    }

    /// <summary>
    /// Gets or sets the event log sequence number of the event.
    /// </summary>
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong EventLogSequenceNumber { get; set; }
}