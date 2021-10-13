// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, Exception> _receivedProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Trace,
                new EventId(281764920, nameof(ReceivedProjection)),
                "Received arguments for projection {Projection} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _registeringProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(75571170, nameof(RegisteringProjection)),
                "Registering stream processor for projection {Projection} on source stream {SourceStream}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringProjection = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(57136794, nameof(ErrorWhileRegisteringProjection)),
                "An error occurred while registering projection {Projection}");

        static readonly Action<ILogger, Guid, Exception> _projectionAlreadyRegistered = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(379156563, nameof(ProjectionAlreadyRegistered)),
                "Failed to register projection {Projection}. It is already registered");

        static readonly Action<ILogger, Guid, Guid, Exception> _startingProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(91153857, nameof(StartingProjection)),
                "Starting projection {ProjectionId} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileStartingProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(1426452095, nameof(ErrorWhileStartingProjection)),
                "An error occurred while starting projection {Projection} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _couldNotStartProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(365915237, nameof(CouldNotStartProjection)),
                "Could not start projection {Projection} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _comparingProjectionDefinition = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(76472302, nameof(ComparingProjectionDefintion)),
                "Comparing projection definition for projection {Projection} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Guid, Exception> _persistingProjectionDefinition = LoggerMessage
            .Define<Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(802598225, nameof(PersistingProjectionDefinition)),
                "Persisting definition of projection {Projection} in scope {Scope} for tenant {Tenant}");

        static readonly Action<ILogger, Guid, Guid, Guid, string, Exception> _resettingProjections = LoggerMessage
            .Define<Guid, Guid, Guid, string>(
                LogLevel.Debug,
                new EventId(260459678, nameof(ResettingProjections)),
                "Resetting projection {Projection} in scope {Scope} for tenant {Tenant} because: {Reason}");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileRunningProjection = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(320592616, nameof(ErrorWhileRunningProjection)),
                "An error occurred while running projection {Projection} in scope {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _eventHandlerDisconnected = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(397656028, nameof(ProjectionDisconnected)),
                "Projection {Projection} in scope {Scope} disconnected");

        static readonly Action<ILogger, Guid, Guid, Guid, Exception> _eventProcessorIsProcessing = LoggerMessage
            .Define<Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(176851418, nameof(EventProcessorIsProcessing)),
                "Projection Event processor {EventProcessor} is processing event type {EventTypeId} for partition {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, uint, string, Exception> _eventProcessorIsProcessingAgain = LoggerMessage
            .Define<Guid, Guid, Guid, uint, string>(
                LogLevel.Debug,
                new EventId(828753281, nameof(EventProcessorIsProcessingAgain)),
                "Projection Event processor {EventProcessor} is processing event type {EventTypeId} for partition {PartitionId} again for the {RetryCount} time, because of {FailureReason}");

        static readonly Action<ILogger, Guid, Guid, Exception> _couldNotGetProjectionKey = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(176305120, nameof(CouldNotGetProjectionKey)),
                "Could not get projection key for projection {Projection} in scope {Scope}");

        internal static void ReceivedProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _receivedProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

        internal static void RegisteringProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _registeringProjection(logger, arguments.ProjectionDefinition.Projection, StreamId.EventLog, null);

        internal static void ErrorWhileRegisteringProjection(this ILogger logger, Exception ex, ProjectionRegistrationArguments arguments)
            => _errorWhileRegisteringProjection(logger, arguments.ProjectionDefinition.Projection, ex);

        internal static void ProjectionAlreadyRegistered(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _projectionAlreadyRegistered(logger, arguments.ProjectionDefinition.Projection, null);

        internal static void StartingProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _startingProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

        internal static void ErrorWhileStartingProjection(this ILogger logger, Exception ex, ProjectionRegistrationArguments arguments)
            => _errorWhileStartingProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, ex);

        internal static void CouldNotStartProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _couldNotStartProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

        internal static void ComparingProjectionDefintion(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _comparingProjectionDefinition(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

        internal static void PersistingProjectionDefinition(this ILogger logger, ProjectionRegistrationArguments arguments, TenantId tenant)
            => _persistingProjectionDefinition(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, tenant, null);

        internal static void ResettingProjections(this ILogger logger, ProjectionRegistrationArguments arguments, TenantId tenant, FailedProjectionDefinitionComparisonReason reason)
            => _resettingProjections(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, tenant, reason, null);

        internal static void ErrorWhileRunningProjection(this ILogger logger, Exception ex, ProjectionRegistrationArguments arguments)
            => _errorWhileRunningProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, ex);

        internal static void ProjectionDisconnected(this ILogger logger, ProjectionRegistrationArguments arguments)
            => _eventHandlerDisconnected(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

        internal static void EventProcessorIsProcessing(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition)
            => _eventProcessorIsProcessing(logger, projectionId, eventType, partition, null);

        internal static void EventProcessorIsProcessingAgain(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition, uint retryCount, string failureReason)
            => _eventProcessorIsProcessingAgain(logger, projectionId, eventType, partition, retryCount, failureReason, null);

        internal static void CouldNotGetProjectionKey(this ILogger logger, EventProcessorId projectionId, ScopeId scope)
            => _couldNotGetProjectionKey(logger, projectionId, scope, null);
    }
}
