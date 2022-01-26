// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

/// <summary>
/// Represents the specification of read model copies to store in MongoDB.
/// </summary>
/// <param name="ShouldCopyToMongoDB">True if copies should be stored in MongoDB, false if not.</param>
/// <param name="Collection">The collection name to store the copies in.</param>
/// <param name="Conversions">The conversions to perform before storing the copies.</param>
public record CopyToMongoDBSpecification(
    bool ShouldCopyToMongoDB,
    CollectionName Collection,
    IDictionary<ProjectionField, ConversionBSONType> Conversions)
{
    /// <summary>
    /// Gets the default value for <see cref="CopyToMongoDBSpecification"/> where no copies will be produced.
    /// </summary>
    public static readonly CopyToMongoDBSpecification Default = new(
        false,
        CollectionName.NotSet,
        ImmutableDictionary<ProjectionField, ConversionBSONType>.Empty);
}