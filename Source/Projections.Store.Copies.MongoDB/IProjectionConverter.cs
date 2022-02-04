// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Defines a system that can convert instance of <see cref="ProjectionState"/> to <see cref="BsonDocument"/> for storing as a read model copy.
/// </summary>
public interface IProjectionConverter
{
    /// <summary>
    /// Converts a <see cref="ProjectionState"/> to a <see cref="BsonDocument"/> using the specified conversions.
    /// </summary>
    /// <param name="state">The projection state to convert.</param>
    /// <param name="conversions">The conversions to apply while converting.</param>
    /// <returns>The converted <see cref="BsonDocument"/>.</returns>
    BsonDocument Convert(ProjectionState state, PropertyConversion[] conversions);
}
