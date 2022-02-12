// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Streams;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Initializing stream processor with id: {StreamProcessorId}")]
    internal static partial void InitializingStreamProcessor(ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Debug, "Starting stream processor with id: {StreamProcessorId}")]
    internal static partial void StartingStreamProcessor(ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Warning, "A failure occurred in a scoped stream processor with id: {StreamProcessorId}")]
    internal static partial void ScopedStreamProcessorFailed(ILogger logger, Exception ex, StreamProcessorId streamProcessorId);
    
    [LoggerMessage(0, LogLevel.Debug, "Stream Processor: {StreamProcessorId} is performing action before setting new position")]
    internal static partial void ScopedStreamProcessorPerformingSetToPositionAction(ILogger logger, IStreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Warning, "Stream Processor: {StreamProcessorId} is failed while performing action before setting new position")]
    internal static partial void ScopedStreamProcessorPerformingSetToPositionActionFailed(ILogger logger, IStreamProcessorId streamProcessorId, Exception exception);

    [LoggerMessage(0, LogLevel.Information, "Stream Processor: {StreamProcessorId} position has been set to {Position}")]
    internal static partial void ScopedStreamProcessorSetToPosition(ILogger logger, IStreamProcessorId streamProcessorId, StreamPosition position);

    [LoggerMessage(0, LogLevel.Warning, "There is already registered a Stream processor with id: {StreamProcessorId}")]
    internal static partial void StreamProcessorAlreadyRegistered(ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Trace, "Successfully registered stream processor with id: {StreamProcessorId}")]
    internal static partial void StreamProcessorSuccessfullyRegistered(ILogger logger, StreamProcessorId streamProcessorId);

    [LoggerMessage(0, LogLevel.Trace, "Unregistering stream processor with id: {StreamProcessorId}")]
    internal static partial void StreamProcessorUnregistered(ILogger logger, StreamProcessorId streamProcessorId);
    
    [LoggerMessage(0, LogLevel.Warning, "{StreamProcessorId} for tenant {TenantId} failed")]
    internal static partial void StreamProcessingForTenantFailed(ILogger logger, Exception ex, IStreamProcessorId streamProcessorId, TenantId tenantId);

    [LoggerMessage(0, LogLevel.Debug, "Event Store is unavailable")]
    internal static partial void EventStoreUnavailable(ILogger logger, EventStoreUnavailable ex);
    
    [LoggerMessage(0, LogLevel.Warning, "Could not persist stream processor state to the event store, will retry in one second.")]
    internal static partial void RetryPersistStreamProcessorState(ILogger logger, Exception ex);
}
