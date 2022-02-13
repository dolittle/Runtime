// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Projections;

/// <summary>
/// Exception that gets thrown when getting all Projections fails.
/// </summary>
public class GetAllProjectionsFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllProjectionsFailed"/> class.
    /// </summary>
    /// <param name="reason">The reason why getting all Projections failed.</param>
    public GetAllProjectionsFailed(string reason)
        : base($"Could not get all projections because {reason}")
    {
    }
}
