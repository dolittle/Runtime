// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB;

/// <summary>
/// Exception that gets thrown when attempting to persist a copy of a Projection read model in MongoDB that should not be stored.
/// </summary>
public class ProjectionShouldNotBeCopiedToMongoDB : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProjectionShouldNotBeCopiedToMongoDB"/> class.
    /// </summary>
    /// <param name="projection">The projection that was attempted to persist a copy for.</param>
    public ProjectionShouldNotBeCopiedToMongoDB(ProjectionDefinition projection)
        : base($"The projection {projection.Projection} in scope {projection.Scope} should not be copied to MongoDB")
    {
    }
}
