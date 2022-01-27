// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Projections.Store.Definition.Copies;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Represents an implementation of <see cref="IProjectionConverter"/>.
/// </summary>
public class ProjectionConverter : IProjectionConverter
{
    /// <inheritdoc />
    public BsonDocument Convert(ProjectionState state, IDictionary<ProjectionField, ConversionBSONType> conversions)
    {
        return BsonDocument.Parse(state);
    }
}
