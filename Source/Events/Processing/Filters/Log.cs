// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Filters;

/// <summary>
/// Log messages for <see cref="Dolittle.Runtime.Events.Processing.Filters"/>.
/// </summary>
static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Filter: {Filter} in scope: {Scope} is filtering event type: {EventType} for partition: {Partition}")]
    internal static partial void FilteringEvent(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition);

    [LoggerMessage(0, LogLevel.Debug, "Filter: {Filter} in scope: {Scope} is filtering event type: {EventType} for partition: {Partition} again for the {RetryCount}. time because: \"{FailureReason}\"")]
    internal static partial void FilteringEventAgain(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition, uint retryCount, FailureReason failureReason);
    
    [LoggerMessage(0, LogLevel.Debug, "Filter: {Filter} in scope: {Scope} is handling filtering result for event type: {EventType} in partition: {Partition}")]
    internal static partial void HandleFilterResult(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition);

    [LoggerMessage(0, LogLevel.Debug, "Filter: {Filter} in scope: {Scope} is writing event type: {EventType} to partition: {Partition} in stream: {Stream}")]
    internal static partial void FilteredEventIsIncluded(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition, StreamId stream);
    
    [LoggerMessage(0, LogLevel.Warning, "Filter: {Filter} in scope: {Scope} failed filtering event type: {EventType}")]
    internal static partial void FailedToFilterEvent(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType);

    [LoggerMessage(0, LogLevel.Trace, "Finding validator for filter: {Filter}")]
    internal static partial void FindingFilterValidator(this ILogger logger, EventProcessorId filter);

    [LoggerMessage(0, LogLevel.Trace, "Filter definition type: {FilterType} can be validated by validator type: {ValidatorType}")]
    internal static partial void FoundValidatorForFilter(this ILogger logger, Type filterType, Type validatorType);

    [LoggerMessage(0, LogLevel.Warning, "There are multiple validators that can validate filter definition of type {FilterType}: {ImplementationTypes}")]
    internal static partial void MultipleValidatorsForFilter(this ILogger logger, Type filterType, IEnumerable<Type> implementationTypes);

    [LoggerMessage(0, LogLevel.Debug, "Validating filter: {Filter} for tenant: {Tenant}")]
    internal static partial void ValidatingFilterForTenant(this ILogger logger, EventProcessorId filter, TenantId tenant);
    
    [LoggerMessage(0, LogLevel.Warning, "Filter registration rejected because execution context is invalid")]
    internal static partial void ExecutionContextIsNotValid(this ILogger logger, Exception exception);

    [LoggerMessage(0, LogLevel.Warning, "Filter id {Filter} is an invalid Stream id")]
    internal static partial void FilterIsInvalid(this ILogger logger, StreamId filter);

    [LoggerMessage(0, LogLevel.Trace, "Received source stream: {SourceStream} filter: {Filter} scope: {Scope}")]
    internal static partial void ReceivedFilter(this ILogger logger, StreamId sourceStream, StreamId filter, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Connecting filter: {Filter}")]
    internal static partial void ConnectingFilter(this ILogger logger, StreamId filter);

    [LoggerMessage(0, LogLevel.Warning, "An error occurred while registering filter: {Filter}")]
    internal static partial void ErrorWhileRegisteringFilter(this ILogger logger, Exception exception, StreamId filter);

    [LoggerMessage(0, LogLevel.Warning, "Failed to register filter: {Filter}. Filter already registered")]
    internal static partial void FilterAlreadyRegistered(this ILogger logger, StreamId filter);

    [LoggerMessage(0, LogLevel.Warning, "An error occurred while starting filter: {Filter} in scope: {Scope}")]
    internal static partial void ErrorWhileStartingFilter(this ILogger logger, Exception exception, StreamId filter, ScopeId scope);

    [LoggerMessage(0, LogLevel.Warning, "Could not start filter: {Filter} in scope: {Scope}")]
    internal static partial void CouldNotStartFilter(this ILogger logger, StreamId filter, ScopeId scope);

    [LoggerMessage(0, LogLevel.Warning, "An error occurred while running filter: {Filter} in scope: {Scope}")]
    internal static partial void ErrorWhileRunningFilter(this ILogger logger, Exception exception, StreamId filter, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Filter: {Filter} in scope: {Scope} stopped")]
    internal static partial void FilterStopped(this ILogger logger, StreamId filter, ScopeId scope);

    [LoggerMessage(0, LogLevel.Debug, "Starting filter: {Filter}")]
    internal static partial void StartingFilter(this ILogger logger, StreamId filter);

    [LoggerMessage(0, LogLevel.Debug, "{FilterType} filter connection request received")]
    internal static partial void FilterConnectionRequestedFor(this ILogger logger, string filterType);

    [LoggerMessage(0, LogLevel.Debug, "Registering stream processor for filter: {Filter} on stream: {SourceStream}")]
    internal static partial void RegisteringStreamProcessorForFilter(this ILogger logger, StreamId filter, StreamId sourceStream);

    [LoggerMessage(0, LogLevel.Debug, "Error occurred while trying to register stream processor for filter: {Filter} on stream: {SourceStream}")]
    internal static partial void ErrorWhileRegisteringStreamProcessorForFilter(this ILogger logger, Exception exception, StreamId filter, StreamId sourceStream);

    [LoggerMessage(0, LogLevel.Trace, "Trying to get find the persisted definition of filter: {Filter} for tenant: {Tenant}")]
    internal static partial void TryGetFilterDefinition(this ILogger logger, EventProcessorId filter, TenantId tenant);

    [LoggerMessage(0, LogLevel.Debug, "Could not get definition of filter: {Filter} for tenant: {Tenant}")]
    internal static partial void NoPersistedFilterDefinition(this ILogger logger, EventProcessorId filter, TenantId tenant);
}
