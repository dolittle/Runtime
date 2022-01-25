// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Projections.Store.Definition;

/// <summary>
/// Represents the definition of a projection.
/// </summary>
/// <param name="Projection">The projection id.</param>
/// <param name="Scope">The scope id.</param>
/// <param name="Events">The list of projection event selectors.</param>
/// <param name="InitialState">The initial projection state.</param>
public record ProjectionDefinition(ProjectionId Projection, ScopeId Scope, IEnumerable<ProjectionEventSelector> Events, ProjectionState InitialState);
