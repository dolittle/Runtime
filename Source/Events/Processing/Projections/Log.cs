// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Projections;

/// <summary>
/// Represents a extensions for <see cref="ILogger" />.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Information, "Connecting Projections")]
    internal static partial void ConnectingProjections(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Received arguments for projection {Projection} in scope {Scope}")]
    internal static partial void ReceivedProjectionArguments(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Warning,"An error occurred while registering projection {Projection} in scope {Scope}" )]
    internal static partial void ErrorWhileRegisteringProjection(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);
    
    [LoggerMessage(0, LogLevel.Information, "Registering projection {Projection} in scope {Scope}")]
    internal static partial void RegisteringProjection(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Warning, "Projection {Projection} in scope {Scope} is already registered")]
    internal static partial void ProjectionAlreadyRegistered(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Debug,"Registering stream processor for projection {Projection} in scope {Scope}." )]
    internal static partial void RegisteringStreamProcessorForProjection(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to register stream processor for projection {Projection} in scope {Scope} is already registered")]
    internal static partial void FailedToRegisterProjectionStreamProcessor(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);
    
    [LoggerMessage(0, LogLevel.Warning, "Failed to get projection key for event occurred key selector type with occurred format {OccurredFormat}")]
    internal static partial void FailedToGetProjectionKeyFromOccurredKeySelector(ILogger logger, Exception exception, OccurredFormat occurredFormat);
    
    [LoggerMessage(0, LogLevel.Information, "Resetting projection {Projection} in scope {Scope} for tenant {Tenant} because: {Reason}" )]
    internal static partial void ResettingProjection(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant, FailedProjectionDefinitionComparisonReason reason);
    
    [LoggerMessage(0, LogLevel.Debug, "Resetting stream processor for projection {Projection} in scope {Scope} for tenant {Tenant}" )]
    internal static partial void ResettingStreamProcessorForTenant(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant);
        
    [LoggerMessage(0, LogLevel.Debug, "Dropping states for projection {Projection} in scope {Scope} for tenant {Tenant}" )]
    internal static partial void DroppingStatesForTenant(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Debug, "Projection {Projection} in scope {Scope} disconnected")]
    internal static partial void ProjectionDisconnected(ILogger logger, ScopeId scope, ProjectionId projection);

    [LoggerMessage(0, LogLevel.Error, "An error occurred while running projection {Projection} in scope {Scope}")]
    internal static partial void ErrorWhileRunningProjection(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);
    
    [LoggerMessage(0, LogLevel.Debug,"Comparing projection definition for projection {Projection} in scope {Scope}" )]
    internal static partial void ComparingProjectionDefiniton(ILogger logger, ScopeId scope, ProjectionId projection);
    
    [LoggerMessage(0, LogLevel.Debug,"Persisting definition of projection {Projection} in scope {Scope} for tenant {Tenant}" )]
    internal static partial void PersistingProjectionDefinition(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Debug,"Starting projection {Projection} in scope {Scope}" )]
    internal static partial void StartingProjection(ILogger logger, ScopeId scope, ProjectionId projection);

    [LoggerMessage(0, LogLevel.Warning,"An error occurred while starting projection {Projection} in scope {Scope}" )]
    internal static partial void ErrorWhileStartingProjection(ILogger logger, ScopeId scope, ProjectionId projection, Exception exception);

    [LoggerMessage(0, LogLevel.Trace, "Projection Event processor {EventProcessor} is processing event type {EventType} for partition {Partition}")]
    internal static partial void EventProcessorIsProcessing(ILogger logger, EventProcessorId eventProcessor, ArtifactId eventType, PartitionId partition);

    [LoggerMessage(0, LogLevel.Trace, "Projection Event processor {EventProcessor} is processing event type {EventType} for partition {Partition} again for the {RetryCount} time, because of {FailureReason}")]
    internal static partial void EventProcessorIsProcessingAgain(ILogger logger, EventProcessorId eventProcessor, ArtifactId eventType, PartitionId partition, uint retryCount, string failureReason);

    [LoggerMessage(0, LogLevel.Debug, "Could not get projection key for projection {Projection} in scope {Scope} for event at {EventLogPosition}")]
    internal static partial void CouldNotGetProjectionKey(ILogger logger, EventProcessorId projection, ScopeId scope, EventLogSequenceNumber eventLogPosition);
    
    [LoggerMessage(0, LogLevel.Information,"Replaying events for projection {Projection} in scope {Scope} for tenant {Tenant}" )]
    internal static partial void ReplayingEventsForTenant(ILogger logger, ScopeId scope, ProjectionId projection, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Information,"Replaying events for projection {Projection} in scope {Scope} for all tenants" )]
    internal static partial void ReplayingEventsForAllTenants(ILogger logger, ScopeId scope, ProjectionId projection);
}
