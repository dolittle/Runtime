// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Conventions;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.Filters
{

    /// <summary>
    /// Represents our own custom <see cref="IDiscriminatorConvention"/> used to deal with our MongoDB representation of
    /// <see cref="FilterDefinition"/>.
    /// </summary>
    public class FilterDefinitionDiscriminatorConvention : IDiscriminatorConvention
    {
        public string ElementName => "";

        public BsonValue GetDiscriminator(Type nominalType, Type actualType)
        {
            throw new NotImplementedException();
        }

        public Type GetActualType(IBsonReader bsonReader, Type nominalType)
        {
            throw new NotImplementedException();
        }
    }
}
