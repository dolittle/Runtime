// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.CLI.Runtime.Configuration;

/// <summary>
/// Exception that gets thrown when getting one Aggregate Root fails.
/// </summary>
public class CommandFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandFailed"/> class.
    /// </summary>
    /// <param name="reason">The reason why the getting configuration yamk failed.</param>
    public CommandFailed(string reason)
        : base($"Could not get configuration yaml because {reason}")
    {
    }
}
