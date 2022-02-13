// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Exception that gets thrown when getting one Projection fails.
/// </summary>
public class GetOneProjectionFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOneProjectionFailed"/> class.
    /// </summary>
    /// <param name="scope">The scope of the Projection.</param>
    /// <param name="projection">The id of the Projection.</param>
    /// <param name="reason">The reason why getting the Projection failed.</param>
    public GetOneProjectionFailed(ScopeId scope, ProjectionId projection, string reason)
        : base($"Could not get projection {projection} in scope {scope} because {reason}")
    {
    }
}
