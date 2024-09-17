// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Keeps the offset metadata for an event log.
/// </summary>
public class EventLogMetadata
{
    [BsonId] public required Guid Scope { get; init; }
    
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong NextEventOffset { get; init; }
}
