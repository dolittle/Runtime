// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Legacy;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

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
    /// <param name="typeId">The id of the event artifact type.</param>
    /// <param name="typeGeneration">The generation of the event artifact.</param>
    /// <param name="isPublic">Whether the Event is public.</param>
    public EventMetadata(
        DateTime occurred,
        string eventSource,
        Guid typeId,
        uint typeGeneration,
        bool isPublic)
    {
        Occurred = occurred;
        EventSource = eventSource;
        TypeId = typeId;
        TypeGeneration = typeGeneration;
        Public = isPublic;
    }

    /// <summary>
    /// Gets or sets the <see cref="DateTime"/> of when the event was committed to the event store with Kind of UTC.
    /// </summary>
    [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime Occurred { get; set; }

    /// <summary>
    /// Gets or sets the event source id.
    /// </summary>
    [BsonSerializer(typeof(EventSourceAndPartitionSerializer))]
    public string EventSource { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ArtifactId"/> of the <see cref="Artifact"/> identitying the type of the event.
    /// </summary>
    public Guid TypeId { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="ArtifactGeneration"/> of the <see cref="Artifact"/> identifying the type of the event.
    /// </summary>
    [BsonRepresentation(BsonType.Int64)]
    public uint TypeGeneration { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this is a public Event.
    /// </summary>
    public bool Public { get; set; }
}
