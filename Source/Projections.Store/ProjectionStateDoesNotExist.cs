// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Projections.Store;

/// <summary>
/// Exception that gets thrown when a <see cref="ProjectionState" /> does not exist.
/// </summary>
public class ProjectionStateDoesNotExist : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="ProjectionStateDoesNotExist" /> class.
    /// </summary>
    /// <param name="projection">The projection id.</param>
    /// <param name="key">The projection key.</param>
    /// <param name="scope">The scope id.</param>
    public ProjectionStateDoesNotExist(ProjectionId projection, ProjectionKey key, ScopeId scope)
        : base($"A projection state for projection {projection.Value} with key {key.Value} in scope {scope.Value} does not exist")
    {
    }
}