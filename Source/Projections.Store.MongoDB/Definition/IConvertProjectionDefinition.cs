// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Defines a system that can convert to an from persisted and runtime representation of a projection definition.
/// </summary>
public interface IConvertProjectionDefinition
{
    ProjectionDefinition ToStored(Store.Definition.ProjectionDefinition definition);
    Store.Definition.ProjectionDefinition ToRuntime(
        ProjectionId projection,
        ScopeId scope,
        IEnumerable<ProjectionEventSelector> eventSelectors,
        Store.State.ProjectionState initialState);

}