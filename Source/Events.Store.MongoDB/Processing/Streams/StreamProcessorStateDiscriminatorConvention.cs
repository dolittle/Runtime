// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams;

/// <summary>
/// Represents our own custom <see cref="IDiscriminatorConvention"/> used to deal with our MongoDB representation of
/// <see cref="StreamProcessorState"/> and <see cref="Partitioned.StreamProcessorStateExtensions"/>.
/// </summary>
/// <remarks>
/// This class was based mostly on information I gathered from these 2 sources:
/// https://groups.google.com/forum/#!topic/mongodb-user/iOeEXbUYbo4
/// https://github.com/mongodb/mongo-csharp-driver/blob/6b73e381827f83af368a949a807076dad01607fc/MongoDB.DriverUnitTests/Samples/MagicDiscriminatorTests.cs#L53 .
/// </remarks>
class StreamProcessorStateDiscriminatorConvention : IDiscriminatorConvention
{
    public string ElementName => "Partitioned";

    /// <inheritdoc/>
    public BsonValue GetDiscriminator(Type nominalType, Type actualType) =>
        actualType == typeof(Partitioned.PartitionedStreamProcessorState);

    /// <inheritdoc/>
    public Type GetActualType(IBsonReader bsonReader, Type nominalType)
    {
        ThrowIfNominalTypeIsIncorrect(nominalType);
        var bookmark = bsonReader.GetBookmark();
        bsonReader.ReadStartDocument();
        ObjectId id = default;
        while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
        {
            var fieldName = bsonReader.ReadName();
            if (fieldName == ElementName)
            {
                var partitioned = bsonReader.ReadBoolean();
                bsonReader.ReturnToBookmark(bookmark);
                return partitioned ? typeof(Partitioned.PartitionedStreamProcessorState) : typeof(StreamProcessorState);
            }
            else if (fieldName == "_id")
            {
                id = bsonReader.ReadObjectId();
            }
            else
            {
                bsonReader.SkipValue();
            }
        }

        bsonReader.ReturnToBookmark(bookmark);
        throw new StreamProcessorStateDocumentIsMissingPartitionedField(id);
    }

    void ThrowIfNominalTypeIsIncorrect(Type nominalType)
    {
        if (!typeof(AbstractStreamProcessorState).IsAssignableFrom(nominalType))
        {
            throw new UnsupportedTypeForStreamProcessorStateDiscriminatorConvention(nominalType);
        }
    }
}