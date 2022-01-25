// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents the persisted copy to MongoDB specification of a projection.
/// </summary>
public class ProjectionCopyToMongoDB
{
    /// <summary>
    /// Gets or sets a value indicating whether or not to produce copies in a MongoDB collection.
    /// </summary>
    public bool ShouldCopyToMongoDB { get; set; }
    
    /// <summary>
    /// Gets or sets the collection name in MongoDB to copy documents into.
    /// </summary>
    public string CollectionName { get; set; }
    
    /// <summary>
    /// Gets or sets the conversions to perform before writing documents to MongoDB.
    /// </summary>
    public IDictionary<string, ConversionBSONType> Conversions { get; set; }
}
