// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "Fast scoped event handler is not supported yet")]
    internal static partial void FastScopedEventHandlerNotSupported(this ILogger logger, Exception exception);
    
    [LoggerMessage(0, LogLevel.Debug, "Connecting Event Handler {EventHandler}")]
    internal static partial void ConnectingEventHandlerWithId(this ILogger logger, EventProcessorId eventHandler);
    
    [LoggerMessage(0, LogLevel.Debug, "Connecting Event Handler")]
    internal static partial void ConnectingEventHandler(this ILogger logger);

    [LoggerMessage(0, LogLevel.Warning, "Event Handler {EventHandler} already registered")]
    internal static partial void EventHandlerAlreadyRegistered(this ILogger logger, EventHandlerId eventHandler);
    
    [LoggerMessage(0, LogLevel.Warning, "{StreamProcessorId} for tenant {TenantId} failed")]
    internal static partial void FilterStreamProcessingForTenantFailed(this ILogger logger, Exception ex, IStreamProcessorId streamProcessorId, TenantId tenantId);
    
    [LoggerMessage(0, LogLevel.Debug, "Initializing stream processor with id: {StreamProcessorId}")]
    internal static partial void InitializingFilterStreamProcessor(this ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Debug, "Starting stream processor with id: {StreamProcessorId}")]
    internal static partial void StartingFilterStreamProcessor(this ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Warning, "A failure occurred in a scoped stream processor with id: {StreamProcessorId}")]
    internal static partial void ScopedFilterStreamProcessorFailed(this ILogger logger, Exception ex, StreamProcessorId streamProcessorId);
    
    [LoggerMessage(0, LogLevel.Warning, "There is already registered a Stream processor with id: {StreamProcessorId}")]
    internal static partial void FilterStreamProcessorAlreadyRegistered(this ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Trace, "Successfully registered stream processor with id: {StreamProcessorId}")]
    internal static partial void FilterStreamProcessorSuccessfullyRegistered(this ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Trace, "Unregistering stream processor with id: {StreamProcessorId}")]
    internal static partial void FilterStreamProcessorUnregistered(this ILogger logger, StreamProcessorId streamProcessorId);
}
