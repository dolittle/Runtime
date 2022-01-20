// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Events.Processing.Projections.for_CompareProjectionDefinitionsForAllTenants.given;

public class projection_definition_builder
{
    public static projection_definition_builder create(ProjectionId projection, ScopeId scope) => new(projection, scope);
    readonly List<ProjectionEventSelector> selectors = new();
    readonly ProjectionId _projection;
    readonly ScopeId _scope;
    ProjectionState _initial_state = string.Empty;

    projection_definition_builder(ProjectionId projection, ScopeId scope)
    {
        _projection = projection;
        _scope = scope;
    }

    public projection_definition_builder with_selector(ProjectionEventSelector selector)
    {
        selectors.Add(selector);
        return this;
    }

    public projection_definition_builder with_initial_state(ProjectionState state)
    {
        _initial_state = state;
        return this;
    }

    public ProjectionDefinition build() => new(_projection, _scope, selectors, _initial_state);
}