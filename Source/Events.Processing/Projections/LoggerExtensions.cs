// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, Exception> _receivedProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Trace,
                new EventId(281764920, nameof(ReceivedProjection)),
                "Received arguments for projection {Projection} with scope: {Scope} ...");

        static readonly Action<ILogger, Guid, Exception> _eventHandlerIsInvalid = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(1224334334, nameof(ProjectionIsInvalid)),
                "Cannot register Projection: {Projection} because it is an invalid projection identifier");

        static readonly Action<ILogger, Guid, Exception> _connectingProjection = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(247861482, nameof(ConnectingProjection)),
                "Connecting projection: {ProjectionId}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringProjection = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(57136794, nameof(ErrorWhileRegisteringProjection)),
                "An error occurred while registering projection: {Projection}");

        static readonly Action<ILogger, Guid, Exception> _handlerAlreadyRegistered = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(379156563, nameof(ProjectionAlreadyRegistered)),
                "Failed to register projection: {Projection}. It is already registered");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileStartingProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(1426452095, nameof(ErrorWhileStartingProjection)),
                "An error occurred while starting projection: {Projection} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _couldNotStartProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(365915237, nameof(CouldNotStartProjection)),
                "Could not start projection: {Projection} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileRunningProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(320592616, nameof(ErrorWhileRunningProjection)),
                "An error occurred while running projection: {Projection} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _eventHandlerDisconnected = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(397656028, nameof(ProjectionDisconnected)),
                "Projection: {Projection} in scope: {Scope} disconnected");

        static readonly Action<ILogger, Guid, Exception> _startingProjection = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(91153857, nameof(StartingProjection)),
                "Starting projection: {ProjectionId}");

        static readonly Action<ILogger, Guid, Guid, Exception> _registeringStreamProcessorForEventProcessor = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(75571170, nameof(RegisteringStreamProcessorForEventProcessor)),
                "Registering stream processor for event processor: {EventProcessor} on source stream: {SourceStream}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringStreamProcessorForEventProcessor = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(418293142, nameof(ErrorWhileRegisteringStreamProcessorForEventProcessor)),
                "Error occurred while trying to register stream processor for Event Processor: {EventProcessor}");

        static readonly Action<ILogger, Guid, Guid, Guid, Exception> _eventProcessorIsProcessing = LoggerMessage
            .Define<Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(176851418, nameof(EventProcessorIsProcessing)),
                "Event processor: {EventProcessor} is processing event type: {EventTypeId} for partition: {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, uint, string, Exception> _eventProcessorIsProcessingAgain = LoggerMessage
            .Define<Guid, Guid, Guid, uint, string>(
                LogLevel.Debug,
                new EventId(828753281, nameof(EventProcessorIsProcessingAgain)),
                "Event processor: {EventProcessor} is processing event type: {EventTypeId} for partition: {PartitionId} again for the {RetryCount} time, because of: {FailureReason}");

        internal static void ReceivedProjection(this ILogger logger, EventProcessorId projection, ScopeId scope)
            => _receivedProjection(logger, projection, scope, null);

        internal static void ProjectionIsInvalid(this ILogger logger, EventProcessorId projection)
            => _eventHandlerIsInvalid(logger, projection, null);

        internal static void ConnectingProjection(this ILogger logger, EventProcessorId projection)
            => _connectingProjection(logger, projection, null);

        internal static void ErrorWhileRegisteringProjection(this ILogger logger, Exception ex, EventProcessorId projectionId)
            => _errorWhileRegisteringProjection(logger, projectionId, ex);

        internal static void ProjectionAlreadyRegistered(this ILogger logger, EventProcessorId projectionId)
            => _handlerAlreadyRegistered(logger, projectionId, null);

        internal static void ErrorWhileStartingProjection(this ILogger logger, Exception ex, StreamId streamId, ScopeId scopeId)
            => _errorWhileStartingProjection(logger, streamId, scopeId, ex);

        internal static void ErrorWhileStartingProjection(this ILogger logger, Exception ex, EventProcessorId projectionId, ScopeId scopeId)
            => _errorWhileStartingProjection(logger, projectionId, scopeId, ex);

        internal static void CouldNotStartProjection(this ILogger logger, EventProcessorId projectionId, ScopeId scopeId)
            => _couldNotStartProjection(logger, projectionId, scopeId, null);

        internal static void ErrorWhileRunningProjection(this ILogger logger, Exception ex, EventProcessorId projectionId, ScopeId scopeId)
            => _errorWhileRunningProjection(logger, projectionId, scopeId, ex);

        internal static void ProjectionDisconnected(this ILogger logger, EventProcessorId projectionId, ScopeId scopeId)
            => _eventHandlerDisconnected(logger, projectionId, scopeId, null);

        internal static void StartingProjection(this ILogger logger, EventProcessorId projectionId)
            => _startingProjection(logger, projectionId, null);

        internal static void RegisteringStreamProcessorForEventProcessor(this ILogger logger, EventProcessorId projectionId, StreamId sourceStream)
            => _registeringStreamProcessorForEventProcessor(logger, projectionId, sourceStream, null);

        internal static void ErrorWhileRegisteringStreamProcessorForEventProcessor(this ILogger logger, Exception ex, EventProcessorId projectionId)
            => _errorWhileRegisteringStreamProcessorForEventProcessor(logger, projectionId, ex);

        internal static void EventProcessorIsProcessing(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition)
            => _eventProcessorIsProcessing(logger, projectionId, eventType, partition, null);

        internal static void EventProcessorIsProcessingAgain(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition, uint retryCount, string failureReason)
            => _eventProcessorIsProcessingAgain(logger, projectionId, eventType, partition, retryCount, failureReason, null);
    }
}
