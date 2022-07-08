// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Projections.Store.MongoDB.Definition;

/// <summary>
/// Represents the persisted copy specification of a projection.
/// </summary>
public class ProjectionCopies
{
    /// <summary>
    /// Gets or sets the copy to MongoDB specification.
    /// </summary>
    public ProjectionCopyToMongoDB MongoDB { get; set; }
}
