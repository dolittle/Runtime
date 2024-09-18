// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events;

/// <summary>
/// Keeps the offset metadata for a specific stream.
/// </summary>
public class StreamMetadata
{
    [BsonId] public required string StreamName { get; init; }
    
    [BsonRepresentation(BsonType.Decimal128)]
    public ulong NextEventOffset { get; init; }
}
