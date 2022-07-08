// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Exception that gets thrown when reprocessing all events fails.
/// </summary>
public class ReprocessAllEventsFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReprocessAllEventsFailed"/> class.
    /// </summary>
    /// <param name="reason">The reason why the reprocessing failed.</param>
    public ReprocessAllEventsFailed(string reason)
        : base($"Could not reprocess all events because {reason}")
    {
    }
}