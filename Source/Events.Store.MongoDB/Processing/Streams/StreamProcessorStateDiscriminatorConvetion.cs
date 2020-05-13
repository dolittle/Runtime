// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.Streams
{
    /// <summary>
    /// Represents our own custom <see cref="IDiscriminatorConvention"/> used to deal with our MongoDB representation of
    /// <see cref="StreamProcessorState"/> and <see cref="Partitioned.StreamProcessorStateExtensions"/>.
    /// This way we skip the need for the '_t' field in the StreamProcessorState collection.
    /// </summary>
    /// <remarks>
    /// This class was based mostly on information I gathered from these 2 sources:
    /// https://groups.google.com/forum/#!topic/mongodb-user/iOeEXbUYbo4
    /// https://github.com/mongodb/mongo-csharp-driver/blob/6b73e381827f83af368a949a807076dad01607fc/MongoDB.DriverUnitTests/Samples/MagicDiscriminatorTests.cs#L53 .
    /// </remarks>
    class StreamProcessorStateDiscriminatorConvetion : IDiscriminatorConvention
    {
        public string ElementName => null;

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            var bookmark = bsonReader.GetBookmark();
            bsonReader.ReadStartDocument();
            var actualType = nominalType;
            while (bsonReader.ReadBsonType() != BsonType.EndOfDocument)
            {
                var fieldName = bsonReader.ReadName();
                if (fieldName == "Partitioned")
                {
                    var partitioned = bsonReader.ReadBoolean();
                    if (partitioned)
                    {
                        actualType = typeof(Partitioned.PartitionedStreamProcessorState);
                        break;
                    }
                    else
                    {
                        actualType = typeof(StreamProcessorState);
                        break;
                    }
                }

                bsonReader.SkipValue();
            }

            bsonReader.ReturnToBookmark(bookmark);
            return actualType;
        }

        public BsonValue GetDiscriminator(Type nominalType, Type actualType) => null;
    }
}
