// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Dolittle.Runtime.Events.Store.MongoDB.Legacy;

/// <summary>
/// Represents an implementation of <see cref="SerializerBase{TValue}"/> that can read either a GUID or a String, and writes as GUID if the value is convertible to a GUID.
/// </summary>
/// <remarks>
/// This serializer was introduced to enable backwards-compatibility in the persisted Event Store from v8 to v6 and v7.
/// </remarks>
public class EventSourceAndPartitionSerializer : SerializerBase<string>
{
    /// <inheritdoc />
    public override string Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var bsonType = context.Reader.CurrentBsonType;
        return bsonType switch
        {
            BsonType.String => context.Reader.ReadString(),
            BsonType.Binary => context.Reader.ReadBinaryData().AsGuid.ToString(),
            _ => throw new BsonSerializationException($"Could not serialize string or guid field to string. BsonType was {bsonType}, expected {BsonType.String} or {BsonType.Binary}")
        };
    }

    /// <inheritdoc />
    public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, string value)
    {
        if (Guid.TryParse(value, out var guid))
        {
            context.Writer.WriteBinaryData(new BsonBinaryData(guid, GuidRepresentation.Standard));
        }
        else
        {
            context.Writer.WriteString(value);
        }
    }
}
