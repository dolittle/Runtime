// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.EventTypes;

/// <summary>
/// Exception that gets thrown when getting all Event Types fails.
/// </summary>
public class GetAllFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetAllFailed"/> class.
    /// </summary>
    /// <param name="reason">The reason why the getting all Event Types failed.</param>
    public GetAllFailed(string reason)
        : base($"Could not get all event types because {reason}")
    {
    }
}