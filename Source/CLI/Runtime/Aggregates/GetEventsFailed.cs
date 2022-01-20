// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Exception that gets thrown when getting events fails.
/// </summary>
public class GetEventsFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetEventsFailed"/> class.
    /// </summary>
    /// <param name="reason">The reason why getting events failed.</param>
    public GetEventsFailed(string reason)
        : base($"Could not get events because {reason}")
    {
    }
}