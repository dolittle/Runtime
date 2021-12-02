// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Connecting Event Handler {EventHandler}")]
    internal static partial void ConnectingEventHandlerWithId(ILogger logger, EventProcessorId eventHandler);
    
    [LoggerMessage(0, LogLevel.Debug, "Connecting Event Handler")]
    internal static partial void ConnectingEventHandler(ILogger logger);
    
}
