// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Lifecycle;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents an implementation of <see cref="IConvertProjectionDefinition" />.
/// </summary>
[Singleton]
public class ConvertProjectionDefinition : IConvertProjectionDefinition
{
    public Store.Definition.ProjectionDefinition ToRuntime(
        ProjectionId projection,
        ScopeId scope,
        IEnumerable<ProjectionEventSelector> eventSelectors,
        Store.State.ProjectionState initialState)
        => new(
            projection,
            scope,
            eventSelectors.Select(_ => new Store.Definition.ProjectionEventSelector(
                _.EventType,
                _.EventKeySelectorType,
                _.EventKeySelectorExpression)),
            initialState);
    public ProjectionDefinition ToStored(Store.Definition.ProjectionDefinition definition)
        => new()
        {
            Projection = definition.Projection,
            InitialStateRaw = definition.InitialState,
            InitialState = BsonDocument.Parse(definition.InitialState),
            EventSelectors = definition.Events.Select(_ => new ProjectionEventSelector
            {
                EventKeySelectorType = _.KeySelectorType,
                EventKeySelectorExpression = _.KeySelectorExpression,
                EventType = _.EventType,
            }).ToArray()
        };
}