// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents the persisted definition of a projection.
/// </summary>
public class ProjectionDefinition
{
    /// <summary>
    /// Gets or sets the projection id.
    /// </summary>
    [BsonId]
    public Guid Projection { get; set; }

    /// <summary>
    /// Gets or sets the initial state.
    /// </summary>
    public BsonDocument InitialState { get; set; }

    /// <summary>
    /// Gets or sets the raw initial state.
    /// </summary>
    public string InitialStateRaw { get; set; }

    /// <summary>
    /// Gets or sets the projection event selectors.
    /// </summary>
    public ProjectionEventSelector[] EventSelectors { get; set; }
}