// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Processing.EventHandlers;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Management.EventHandlers;

/// <summary>
/// Represents log messages for Event Handlers management.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Received Event Handler Management GetAll request.")]
    internal static partial void GetAll(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Information, "Received Event Handler Management GetOne request for event handler {EventHandler} in scope {Scope}.")]
    internal static partial void GetOne(ILogger logger, EventProcessorId eventHandler, ScopeId scope);
    
    [LoggerMessage(0, LogLevel.Warning, "Event Handler {EventHandler} in scope {Scope} is not registered.")]
    internal static partial void EventHandlerNotRegistered(ILogger logger, EventProcessorId eventHandler, ScopeId scope);

    [LoggerMessage(0, LogLevel.Information, "Received Event Handler Management ReprocessEventsFrom request for event handler {EventHandler} in scope {Scope} for {Tenant} from position {Position}.")]
    internal static partial void ReprocessEventsFrom(ILogger logger, EventProcessorId eventHandler, ScopeId scope, TenantId tenant, StreamPosition position);

    [LoggerMessage(0, LogLevel.Information, "Received Event Handler Management ReprocessAllEvents request for event handler {EventHandler} in scope {Scope} for all tenants.")]
    internal static partial void ReprocessAllEvents(ILogger logger, EventProcessorId eventHandler, ScopeId scope);

    [LoggerMessage(0, LogLevel.Error, "Failed to reprocess events for Event Handler")]
    internal static partial void FailedDuringReprocessing(ILogger logger, Exception exception);
}
