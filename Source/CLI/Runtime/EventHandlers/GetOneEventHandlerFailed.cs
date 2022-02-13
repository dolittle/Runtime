// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.EventHandlers;

namespace Dolittle.Runtime.CLI.Runtime.EventHandlers;

/// <summary>
/// Exception that gets thrown when getting one Event Handler fails.
/// </summary>
public class GetOneEventHandlerFailed : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetOneEventHandlerFailed"/> class.
    /// </summary>
    /// <param name="eventHandler">The Event Handler that getting one failed for.</param>
    /// <param name="reason">The reason why getting one Event Handler failed.</param>
    public GetOneEventHandlerFailed(EventHandlerId eventHandler, string reason)
        : base($"Could not get event handler {eventHandler.EventHandler} in scope {eventHandler.Scope} because {reason}")
    {
    }
}
