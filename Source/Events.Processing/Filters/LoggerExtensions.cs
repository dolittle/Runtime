// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Guid, Guid, string, Exception> _filteringEvent = LoggerMessage
            .Define<Guid, Guid, Guid, string>(
                LogLevel.Debug,
                new EventId(568531282, nameof(FilteringEvent)),
                "Filter: {Filter} in scope: {Scope} is filtering event type: {EventTypeId} for partition: {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, string, uint, string, Exception> _filteringEventAgain = LoggerMessage
            .Define<Guid, Guid, Guid, string, uint, string>(
                LogLevel.Debug,
                new EventId(391135754, nameof(FilteringEventAgain)),
                "Filter: {Filter} in scope: {Scope} is filtering event type: {EventTypeId} for partition: {PartitionId} again for the {RetryCount}. time because: \"{FailureReason}\"");

        static readonly Action<ILogger, Guid, Guid, Guid, string, Exception> _handleFilterResult = LoggerMessage
            .Define<Guid, Guid, Guid, string>(
                LogLevel.Debug,
                new EventId(1502330189, nameof(HandleFilterResult)),
                "Filter: {Filter} in scope: {Scope} is handling filtering result for event type: {EventTypeId} in partition: {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, string, Guid, Exception> _filteredEventIsIncluded = LoggerMessage
            .Define<Guid, Guid, Guid, string, Guid>(
                LogLevel.Debug,
                new EventId(1603417156, nameof(FilteredEventIsIncluded)),
                "Filter: {Filter} in scope: {Scope} is writing event type: {EventTypeId} to partition: {PartitionId} in stream: {Stream}");

        static readonly Action<ILogger, Guid, Exception> _findingFilterValidator = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(255775676, nameof(FindingFilterValidator)),
                "Finding validator for filter: {Filter}");

        static readonly Action<ILogger, Type, Type, Exception> _foundValidatorForFilter = LoggerMessage
            .Define<Type, Type>(
                LogLevel.Trace,
                new EventId(362042910, nameof(FoundValidatorForFilter)),
                "Filter definition type: {FilterType} can be validated by validator type: {ValidatorType}");
        static readonly Action<ILogger, Type, string, Exception> _multipleValidatorsForFilter = LoggerMessage
            .Define<Type, string>(
                LogLevel.Warning,
                new EventId(342051795, nameof(MultipleValidatorsForFilter)),
                "There are multiple validators that can validate filter definition of type: {FilterDefinitionType}:\n{ImplementationTypes}\nUsing the first validator");

        static readonly Action<ILogger, Guid, Guid, Exception> _validatingFilterForTenant = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(1631987633, nameof(ValidatingFilterForTenant)),
                "Validating filter: {Filter} for tenant: {Tenant}");


        static readonly Action<ILogger, Guid, Exception> _filterIsInvalid = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(272919992, nameof(FilterIsInvalid)),
                "Filter: {Filter} is an invalid Stream Id");

        static readonly Action<ILogger, Guid, Guid, Guid, Exception> _receivedFilter = LoggerMessage
            .Define<Guid, Guid, Guid>(
                LogLevel.Trace,
                new EventId(1743024517, nameof(ReceivedFilter)),
                "Received source stream: {SourceStream} filter: {Filter} scope: {Scope}");

        static readonly Action<ILogger, Guid, Exception> _connectingFilter = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(1595647774, nameof(ConnectingFilter)),
                "Connecting filter: {Filter}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileRegisteringFilter = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(1554818652, nameof(ErrorWhileRegisteringFilter)),
                "An error occurred while registering filter: {Filter}");

        static readonly Action<ILogger, Guid, Exception> _filterAlreadyRegistered = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(791629241, nameof(FilterAlreadyRegistered)),
                "Failed to register filter: {Filter}. Filter already registered");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileStartingFilter = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(1853029746, nameof(ErrorWhileStartingFilter)),
                "An error occurred while starting filter: {Filter} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _couldNotStartFilter = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(1853029746, nameof(CouldNotStartFilter)),
                "Could not start filter: {Filter} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileRunningFilter = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Warning,
                new EventId(1853029746, nameof(ErrorWhileRunningFilter)),
                "An error occurred while running filter: {Filter} in scope: {Scope}");

        static readonly Action<ILogger, Guid, Guid, Exception> _filterStopped = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(1011014935, nameof(FilterStopped)),
                "Filter: {Filter} in scope: {Scope} stopped");

        static readonly Action<ILogger, Guid, Exception> _startingFilter = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(1295926017, nameof(StartingFilter)),
                "Starting filter: {Filter}");

        static readonly Action<ILogger, string, Exception> _filterConnectionRequestedFor = LoggerMessage
            .Define<string>(
                LogLevel.Debug,
                new EventId(309326952, nameof(FilterConnectionRequestedFor)),
                "{FilterType} filter connection request received");

        static readonly Action<ILogger, Guid, Guid, Exception> _registeringStreamProcessorForFilter = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(2125123102, nameof(RegisteringStreamProcessorForFilter)),
                "Registering stream processor for filter: {Filter} on stream: {SourceStream}");

        static readonly Action<ILogger, Guid, Guid, Exception> _errorWhileRegisteringStreamProcessorForFilter = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(262022487, nameof(ErrorWhileRegisteringStreamProcessorForFilter)),
                "Error occurred while trying to register stream processor for filter: {Filter} on stream: {SourceStream}");


        static readonly Action<ILogger, Guid, Guid, Exception> _tryGetFilterDefinition = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Trace,
                new EventId(1923819068, nameof(TryGetFilterDefinition)),
                "Trying to get find the persisted definition of filter: {Filter} for tenant: {Tenant}");

        static readonly Action<ILogger, Guid, Guid, Exception> _noPersistedFilterDefinition = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(236207818, nameof(NoPersistedFilterDefinition)),
                "Could not get definition of filter: {Filter} for tenant: {Tenant}");

        internal static void FilteringEvent(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition)
            => _filteringEvent(logger, filter, scope, eventType, partition, null);

        internal static void FilteringEventAgain(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition, uint retryCount, FailureReason failureReason)
            => _filteringEventAgain(logger, filter, scope, eventType, partition, retryCount, failureReason, null);

        internal static void HandleFilterResult(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition)
            => _handleFilterResult(logger, filter, scope, eventType, partition, null);

        internal static void FilteredEventIsIncluded(this ILogger logger, EventProcessorId filter, ScopeId scope, ArtifactId eventType, PartitionId partition, StreamId stream)
            => _filteredEventIsIncluded(logger, filter, scope, eventType, partition, stream, null);

        internal static void FindingFilterValidator(this ILogger logger, EventProcessorId filter)
            => _findingFilterValidator(logger, filter, null);

        internal static void FoundValidatorForFilter(this ILogger logger, Type filterType, Type validatorType)
            => _foundValidatorForFilter(logger, filterType, validatorType, null);

        internal static void MultipleValidatorsForFilter(this ILogger logger, Type filterType, IEnumerable<Type> implementations)
            => _multipleValidatorsForFilter(logger, filterType, string.Join("\n", implementations.Select(_ => _.ToString())), null);

        internal static void ValidatingFilterForTenant(this ILogger logger, EventProcessorId filter, TenantId tenant)
            => _validatingFilterForTenant(logger, filter, tenant, null);

        internal static void FilterIsInvalid(this ILogger logger, StreamId filterId)
            => _filterIsInvalid(logger, filterId, null);

        internal static void ReceivedFilter(this ILogger logger, StreamId sourceStream, StreamId filterId, ScopeId scopeId)
            => _receivedFilter(logger, sourceStream, filterId, scopeId, null);

        internal static void ConnectingFilter(this ILogger logger, StreamId filterId)
            => _connectingFilter(logger, filterId, null);

        internal static void ErrorWhileRegisteringFilter(this ILogger logger, Exception ex, StreamId filterId)
            => _errorWhileRegisteringFilter(logger, filterId, ex);

        internal static void FilterAlreadyRegistered(this ILogger logger, StreamId filterId)
            => _filterAlreadyRegistered(logger, filterId, null);

        internal static void ErrorWhileStartingFilter(this ILogger logger, Exception ex, StreamId filterId, ScopeId scopeId)
            => _errorWhileStartingFilter(logger, filterId, scopeId, ex);

        internal static void CouldNotStartFilter(this ILogger logger, StreamId filterId, ScopeId scopeId)
            => _couldNotStartFilter(logger, filterId, scopeId, null);

        internal static void ErrorWhileRunningFilter(this ILogger logger, Exception ex, StreamId filterId, ScopeId scopeId)
            => _errorWhileRunningFilter(logger, filterId, scopeId, ex);

        internal static void FilterStopped(this ILogger logger, StreamId filterId, ScopeId scopeId)
            => _filterStopped(logger, filterId, scopeId, null);

        internal static void StartingFilter(this ILogger logger, StreamId filterId)
            => _startingFilter(logger, filterId, null);

        internal static void FilterConnectionRequestedFor(this ILogger logger, string filterType)
            => _filterConnectionRequestedFor(logger, filterType, null);

        internal static void RegisteringStreamProcessorForFilter(this ILogger logger, StreamId filterId, StreamId sourceStream)
            => _registeringStreamProcessorForFilter(logger, filterId, sourceStream, null);

        internal static void ErrorWhileRegisteringStreamProcessorForFilter(this ILogger logger, Exception ex, StreamId filterId, StreamId sourceStream)
            => _errorWhileRegisteringStreamProcessorForFilter(logger, filterId, sourceStream, ex);

        internal static void TryGetFilterDefinition(this ILogger logger, EventProcessorId filter, TenantId tenant)
            => _tryGetFilterDefinition(logger, filter, tenant, null);

        internal static void NoPersistedFilterDefinition(this ILogger logger, EventProcessorId filter, TenantId tenant)
            => _noPersistedFilterDefinition(logger, filter, tenant, null);
    }
}
