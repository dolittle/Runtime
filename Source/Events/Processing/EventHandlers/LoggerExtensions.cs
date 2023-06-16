// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.EventHandlers;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class LoggerExtensions
{
    static readonly Action<ILogger, Guid, Guid, Guid, string, bool, Exception> _receivedEventHandler = LoggerMessage
        .Define<Guid, Guid, Guid, string, bool>(
            LogLevel.Trace,
            new EventId(267299009, nameof(ReceivedEventHandler)),
            "Received arguments for event handler {EventHandler} with source stream: {SourceStream} scope: {Scope} filtered on event types: {Types}\nPartitioned: {Partitioned}");

    static readonly Action<ILogger, Guid, Exception> _eventHandlerIsInvalid = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(1712395899, nameof(EventHandlerIsInvalid)),
            "Cannot register event handler: {EventHandler} because it is an invalid stream identifier");

    static readonly Action<ILogger, Guid, Exception> _connectingEventHandler = LoggerMessage
        .Define<Guid>(
            LogLevel.Debug,
            new EventId(352998252, nameof(ConnectingEventHandler)),
            "Connecting event handler: {EventHandlerId}");

    static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringEventHandler = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(231722653, nameof(ErrorWhileRegisteringEventHandler)),
            "An error occurred while registering event handler: {EventHandler}");

    static readonly Action<ILogger, Guid, Exception> _handlerAlreadyRegistered = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(1850293039, nameof(EventHandlerAlreadyRegistered)),
            "Failed to register event Handler: {EventHandler}. It is already registered");

    static readonly Action<ILogger, Guid, Guid, Exception> _handlerAlreadyRegisteredOnSourceStream = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Warning,
            new EventId(780701763, nameof(EventHandlerAlreadyRegisteredOnSourceStream)),
            "Failed to register event handler: {EventHandler}. An event processor is already registered on source stream: {SourceStream}");

    static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileStartingEventHandler = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Warning,
            new EventId(219875179, nameof(ErrorWhileStartingEventHandler)),
            "An error occurred while starting event handler: {EventHandler} in scope: {Scope}");

    static readonly Action<ILogger, Guid, Guid, Exception> _couldNotStartEventHandler = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Warning,
            new EventId(860866540, nameof(CouldNotStartEventHandler)),
            "Could not start event handler: {EventHandler} in scope: {Scope}");

    static readonly Action<ILogger, Guid, Guid, Exception> _eventHandlerDisconnected = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(414482081, nameof(EventHandlerDisconnected)),
            "Event handler: {EventHandler} in scope: {Scope} disconnected");

    static readonly Action<ILogger, Guid, Exception> _startingEventHandler = LoggerMessage
        .Define<Guid>(
            LogLevel.Debug,
            new EventId(667717425, nameof(StartingEventHandler)),
            "Starting event handler: {EventHandlerId}");

    static readonly Action<ILogger, Guid, Exception> _registeringStreamProcessorForFilter = LoggerMessage
        .Define<Guid>(
            LogLevel.Debug,
            new EventId(1337268627, nameof(RegisteringStreamProcessorForFilter)),
            "Registering stream processor for filter {FilterId} on event log");

    static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringStreamProcessorForFilter = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(197581492, nameof(ErrorWhileRegisteringStreamProcessorForFilter)),
            "Error occurred while trying to register stream processor for filter {EventHandler}");

    static readonly Action<ILogger, Guid, Guid, Exception> _registeringStreamProcessorForEventProcessor = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(472572190, nameof(RegisteringStreamProcessorForEventProcessor)),
            "Registering stream processor for event processor: {EventProcessor} on source stream: {SourceStream}");

    static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringStreamProcessorForEventProcessor = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(381357522, nameof(ErrorWhileRegisteringStreamProcessorForEventProcessor)),
            "Error occurred while trying to register stream processor for Event Processor: {EventProcessor}");

    static readonly Action<ILogger, Guid, Guid, string, Exception> _eventProcessorIsProcessing = LoggerMessage
        .Define<Guid, Guid, string>(
            LogLevel.Debug,
            new EventId(265113086, nameof(EventProcessorIsProcessing)),
            "Event processor: {EventProcessor} is processing event type: {EventTypeId} for partition: {PartitionId}");

    static readonly Action<ILogger, Guid, Guid, string, uint, string, Exception> _eventProcessorIsProcessingAgain = LoggerMessage
        .Define<Guid, Guid, string, uint, string>(
            LogLevel.Debug,
            new EventId(250914604, nameof(EventProcessorIsProcessingAgain)),
            "Event processor: {EventProcessor} is processing event type: {EventTypeId} for partition: {PartitionId} again for the {RetryCount} time, because of: {FailureReason}");

    internal static void ReceivedEventHandler(this ILogger logger, StreamId sourceStream, EventProcessorId handler, ScopeId scope,
        IEnumerable<ArtifactId> types, bool partitioned)
        => _receivedEventHandler(logger, sourceStream, handler, scope, string.Join(", ", types.Select(_ => $"'{_.Value}'")), partitioned, null);

    internal static void EventHandlerIsInvalid(this ILogger logger, EventProcessorId handler)
        => _eventHandlerIsInvalid(logger, handler, null);

    internal static void ConnectingEventHandler(this ILogger logger, EventProcessorId handler)
        => _connectingEventHandler(logger, handler, null);

    internal static void ErrorWhileRegisteringEventHandler(this ILogger logger, Exception ex, EventProcessorId handlerId)
        => _errorWhileRegisteringEventHandler(logger, handlerId, ex);

    internal static void EventHandlerAlreadyRegistered(this ILogger logger, EventProcessorId handlerId)
        => _handlerAlreadyRegistered(logger, handlerId, null);

    internal static void EventHandlerAlreadyRegisteredOnSourceStream(this ILogger logger, EventProcessorId handlerId)
        => _handlerAlreadyRegisteredOnSourceStream(logger, handlerId, handlerId, null);

    internal static void ErrorWhileStartingEventHandler(this ILogger logger, Exception ex, StreamId streamId, ScopeId scopeId)
        => _errorWhileStartingEventHandler(logger, streamId, scopeId, ex);

    internal static void ErrorWhileStartingEventHandler(this ILogger logger, Exception ex, EventProcessorId handlerId, ScopeId scopeId)
        => _errorWhileStartingEventHandler(logger, handlerId, scopeId, ex);

    internal static void CouldNotStartEventHandler(this ILogger logger, EventProcessorId handlerId, ScopeId scopeId)
        => _couldNotStartEventHandler(logger, handlerId, scopeId, null);


    [LoggerMessage(0, LogLevel.Warning, "An error occurred while running event handler: {EventHandler} in scope: {Scope}")]
    internal static partial void ErrorWhileRunningEventHandler(this ILogger logger, Exception ex, EventProcessorId eventHandler, ScopeId scope);

    [LoggerMessage(0, LogLevel.Information, "Waiting for completions: {EventHandler} in scope: {Scope}, {RequestsInFlight} requests in flight")]
    internal static partial void WaitingForCompletions(this ILogger logger, EventProcessorId eventHandler, ScopeId scope, int requestsInFlight);
    
    [LoggerMessage(0, LogLevel.Information, "All handlers complete: {EventHandler} in scope: {Scope}")]
    internal static partial void FinishedWaitingForCompletions(this ILogger logger, EventProcessorId eventHandler, ScopeId scope);

    [LoggerMessage(0, LogLevel.Error, "Failed while waiting for completions: {EventHandler} in scope: {Scope}")]
    internal static partial void FailedWaitingForCompletions(this ILogger logger, Exception ex, EventProcessorId eventHandler, ScopeId scope, int requestsInFlight);

    [LoggerMessage(0, LogLevel.Information, "EventHandler was cancelled: {EventHandler} in scope: {Scope}")]
    internal static partial void CancelledRunningEventHandler(this ILogger logger, Exception ex, EventProcessorId eventHandler, ScopeId scope);

    [LoggerMessage(0, LogLevel.Information, "Failing eventHandler was cancelled, no retries scheduled: {EventHandler} in scope: {Scope}. Error: {Reason}")]
    internal static partial void StoppedFailingEventHandler(this ILogger logger, EventProcessorId eventHandler, ScopeId scope, string reason);

    internal static void EventHandlerDisconnected(this ILogger logger, EventProcessorId handlerId, ScopeId scopeId)
        => _eventHandlerDisconnected(logger, handlerId, scopeId, null);

    internal static void StartingEventHandler(this ILogger logger, StreamId streamId)
        => _startingEventHandler(logger, streamId, null);

    internal static void RegisteringStreamProcessorForFilter(this ILogger logger, EventProcessorId handlerId)
        => _registeringStreamProcessorForFilter(logger, handlerId, null);

    internal static void ErrorWhileRegisteringStreamProcessorForFilter(this ILogger logger, Exception ex, EventProcessorId handlerId)
        => _errorWhileRegisteringStreamProcessorForFilter(logger, handlerId, ex);

    internal static void RegisteringStreamProcessorForEventProcessor(this ILogger logger, EventProcessorId handlerId, StreamId sourceStream)
        => _registeringStreamProcessorForEventProcessor(logger, handlerId, sourceStream, null);

    internal static void ErrorWhileRegisteringStreamProcessorForEventProcessor(this ILogger logger, Exception ex, EventProcessorId handlerId)
        => _errorWhileRegisteringStreamProcessorForEventProcessor(logger, handlerId, ex);

    internal static void EventProcessorIsProcessing(this ILogger logger, EventProcessorId handlerId, ArtifactId eventType, PartitionId partition)
        => _eventProcessorIsProcessing(logger, handlerId, eventType, partition, null);

    internal static void EventProcessorIsProcessingAgain(this ILogger logger, EventProcessorId handlerId, ArtifactId eventType, PartitionId partition,
        uint retryCount, string failureReason)
        => _eventProcessorIsProcessingAgain(logger, handlerId, eventType, partition, retryCount, failureReason, null);
}
