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
    /// </summary>
    /// <remarks>
    /// This class was based mostly on information I gathered from these 2 sources:
    /// https://groups.google.com/forum/#!topic/mongodb-user/iOeEXbUYbo4
    /// https://github.com/mongodb/mongo-csharp-driver/blob/6b73e381827f83af368a949a807076dad01607fc/MongoDB.DriverUnitTests/Samples/MagicDiscriminatorTests.cs#L53 .
    /// </remarks>
    class StreamProcessorStateDiscriminatorConvention : IDiscriminatorConvention
    {
        public string ElementName => "Partitioned";

        /// <summary>
        /// Gets the value to be put into the ElementName key when writing <see cref="AbstractStreamProcessorState"/>
        /// into the collection.
        /// </summary>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="actualType">The actual type.</param>
        /// <returns>Boolean indicating whether the StreamProcessorState is partitioned.</returns>
        public BsonValue GetDiscriminator(Type nominalType, Type actualType) =>
            actualType == typeof(Partitioned.PartitionedStreamProcessorState);

        /// <summary>
        /// Gets the correct type when deserializing objects from  <see cref="AbstractStreamProcessorState"/> collection
        /// depending on the given nominal type or "Partitioned" field.
        /// </summary>
        /// <param name="bsonReader">A <see cref="IBsonReader"/>.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <returns>The actual wanted type.</returns>
        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            if (nominalType == typeof(StreamProcessorState))
            {
                return typeof(StreamProcessorState);
            }
            else if (nominalType == typeof(Partitioned.PartitionedStreamProcessorState))
            {
                return typeof(Partitioned.PartitionedStreamProcessorState);
            }
            else if (nominalType == typeof(AbstractStreamProcessorState))
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
                    else
                    {
                        bsonReader.SkipValue();
                    }
                }

                bsonReader.ReturnToBookmark(bookmark);
                return actualType;
            }
            else
            {
                throw new UnsupportedTypeForStreamProcessorStateDiscriminatorConvention(nominalType);
            }
        }
    }
}
