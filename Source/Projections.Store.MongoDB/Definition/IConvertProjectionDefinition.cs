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
    /// <summary>
    /// Converts a <see cref="Store.Definition.ProjectionDefinition"/> to a persisted <see cref="ProjectionDefinition"/>.
    /// </summary>
    /// <param name="definition">The definition to convert.</param>
    /// <returns>The converted persisted definition.</returns>
    ProjectionDefinition ToStored(Store.Definition.ProjectionDefinition definition);
    
    /// <summary>
    /// Creates a <see cref="Store.Definition.ProjectionDefinition"/> from its persisted components.
    /// </summary>
    /// <param name="projection">The persisted projection identifier.</param>
    /// <param name="scope">The persisted scope identifier.</param>
    /// <param name="eventSelectors">The persisted event selectors.</param>
    /// <param name="initialState">The persisted initial state.</param>
    /// <param name="copies">The persisted read model copy specification</param>
    /// <returns>The converted definition.</returns>
    Store.Definition.ProjectionDefinition ToRuntime(
        ProjectionId projection,
        ScopeId scope,
        IEnumerable<ProjectionEventSelector> eventSelectors,
        Store.State.ProjectionState initialState,
        ProjectionCopies copies);
}
