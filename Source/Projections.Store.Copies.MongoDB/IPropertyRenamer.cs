// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Contracts;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Defines a system that can rename properties in a <see cref="BsonDocument"/> according to rules specified in a list of <see cref="ProjectionCopyToMongoDB.Types.PropertyConversion"/>.
/// </summary>
public interface IPropertyRenamer
{
    /// <summary>
    /// Renames properties in a <see cref="BsonDocument"/> using the specified conversions.
    /// </summary>
    /// <param name="document">The document to rename properties in.</param>
    /// <param name="conversions">The conversions that describe properties to rename.</param>
    void RenamePropertiesIn(BsonDocument document, PropertyConversion[] conversions);
}
