// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Dolittle.Runtime.MongoDB.Serialization;

/// <summary>
/// Represents an implementation of <see cref="SerializerBase{TValue}"/> for serializing strings that are guids or string.
/// </summary>
public class StringOrGuidSerializer : SerializerBase<string>
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
