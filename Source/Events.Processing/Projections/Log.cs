// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Connecting Projections")]
    internal static partial void ConnectingProjections(ILogger logger);

    [LoggerMessage(0, LogLevel.Warning, "Projection {Projection} in scope {Scope} is already registered")]
    internal static partial void ProjectionAlreadyRegistered(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to register stream processor for projection {Projection} in scope {Scope} is already registered")]
    internal static partial void FailedToRegisterProjectionStreamProcessor(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);
    
    [LoggerMessage(0, LogLevel.Information, "Resetting projection {Projection} in scope {Scope} for tenant {Tenant} because: {Reason}" )]
    internal static partial void ResettingProjection(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant, FailedProjectionDefinitionComparisonReason reason);

    [LoggerMessage(0, LogLevel.Trace, "Received arguments for projection {Projection} in scope {Scope}")]
    internal static partial void ReceivedProjectionArguments(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Debug, "Projection {Projection} in scope {Scope} disconnected")]
    internal static partial void ProjectionDisconnected(ILogger logger, ScopeId scope, ProjectionId projection);

    [LoggerMessage(0, LogLevel.Error, "An error occurred while running projection {Projection} in scope {Scope}")]
    internal static partial void ErrorWhileRunningProjection(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);
    
    [LoggerMessage(0, LogLevel.Debug,"Persisting definition of projection {Projection} in scope {Scope} for tenant {Tenant}" )]
    internal static partial void PersistingProjectionDefinition(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant);
    
    
    
    
    
    
    static readonly Action<ILogger, Guid, Guid, Exception> _registeringProjection = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(75571170, nameof(RegisteringProjection)),
            "Registering stream processor for projection {Projection} on source stream {SourceStream}");
    
    internal static void RegisteringProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
        => _registeringProjection(logger, arguments.ProjectionDefinition.Projection, StreamId.EventLog, null);

    static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringProjection = LoggerMessage
        .Define<Guid>(
            LogLevel.Warning,
            new EventId(57136794, nameof(ErrorWhileRegisteringProjection)),
            "An error occurred while registering projection {Projection}");
    
    internal static void ErrorWhileRegisteringProjection(this ILogger logger, Exception ex, ProjectionRegistrationArguments arguments)
        => _errorWhileRegisteringProjection(logger, arguments.ProjectionDefinition.Projection, ex);

    static readonly Action<ILogger, Guid, Guid, Exception> _startingProjection = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(91153857, nameof(StartingProjection)),
            "Starting projection {ProjectionId} in scope {Scope}");
    
    internal static void StartingProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
        => _startingProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

    static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileStartingProjection = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Warning,
            new EventId(1426452095, nameof(ErrorWhileStartingProjection)),
            "An error occurred while starting projection {Projection} in scope {Scope}");
    
    internal static void ErrorWhileStartingProjection(this ILogger logger, Exception ex, ProjectionRegistrationArguments arguments)
        => _errorWhileStartingProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, ex);

    static readonly Action<ILogger, Guid, Guid, Exception> _couldNotStartProjection = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Warning,
            new EventId(365915237, nameof(CouldNotStartProjection)),
            "Could not start projection {Projection} in scope {Scope}");
    
    internal static void CouldNotStartProjection(this ILogger logger, ProjectionRegistrationArguments arguments)
        => _couldNotStartProjection(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);

    static readonly Action<ILogger, Guid, Guid, Exception> _comparingProjectionDefinition = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(76472302, nameof(ComparingProjectionDefintion)),
            "Comparing projection definition for projection {Projection} in scope {Scope}");
    
    internal static void ComparingProjectionDefintion(this ILogger logger, ProjectionRegistrationArguments arguments)
        => _comparingProjectionDefinition(logger, arguments.ProjectionDefinition.Projection, arguments.ProjectionDefinition.Scope, null);



    static readonly Action<ILogger, Guid, Guid, string, Exception> _eventProcessorIsProcessing = LoggerMessage
        .Define<Guid, Guid, string>(
            LogLevel.Trace,
            new EventId(176851418, nameof(EventProcessorIsProcessing)),
            "Projection Event processor {EventProcessor} is processing event type {EventTypeId} for partition {PartitionId}");
    
    internal static void EventProcessorIsProcessing(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition)
        => _eventProcessorIsProcessing(logger, projectionId, eventType, partition, null);

    static readonly Action<ILogger, Guid, Guid, string, uint, string, Exception> _eventProcessorIsProcessingAgain = LoggerMessage
        .Define<Guid, Guid, string, uint, string>(
            LogLevel.Debug,
            new EventId(828753281, nameof(EventProcessorIsProcessingAgain)),
            "Projection Event processor {EventProcessor} is processing event type {EventTypeId} for partition {PartitionId} again for the {RetryCount} time, because of {FailureReason}");
    
    internal static void EventProcessorIsProcessingAgain(this ILogger logger, EventProcessorId projectionId, ArtifactId eventType, PartitionId partition, uint retryCount, string failureReason)
        => _eventProcessorIsProcessingAgain(logger, projectionId, eventType, partition, retryCount, failureReason, null);

    static readonly Action<ILogger, Guid, Guid, Exception> _couldNotGetProjectionKey = LoggerMessage
        .Define<Guid, Guid>(
            LogLevel.Debug,
            new EventId(176305120, nameof(CouldNotGetProjectionKey)),
            "Could not get projection key for projection {Projection} in scope {Scope}");
    
    internal static void CouldNotGetProjectionKey(this ILogger logger, EventProcessorId projectionId, ScopeId scope)
        => _couldNotGetProjectionKey(logger, projectionId, scope, null);
}
