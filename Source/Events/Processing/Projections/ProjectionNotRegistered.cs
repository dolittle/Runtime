// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Exception that gets thrown when attempting to perform an action on a Projection that not registered.
/// </summary>
public class ProjectionNotRegistered : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionNotRegistered"/> class.
    /// </summary>
    /// <param name="scope">The scope of the Projection that is not registered.</param>
    /// <param name="projection">The id of the Projection that is not registered.</param>
    public ProjectionNotRegistered(ScopeId scope, ProjectionId projection)
        : base($"The projection {projection} in scope {scope} is not registered")
    {
    }
}
