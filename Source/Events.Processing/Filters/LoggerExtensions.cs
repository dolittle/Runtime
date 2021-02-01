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
        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Exception> _filteringEvent = LoggerMessage
            .Define<Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(568531282, nameof(FilteringEvent)),
                "Filter {Filter} in scope {Scope} is filtering event {EventTypeId} for partition {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, uint, string, Exception> _filteringEventAgain = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, uint, string>(
                LogLevel.Debug,
                new EventId(391135754, nameof(FilteringEventAgain)),
                "Filter {Filter} in scope {Scope} is filtering event {EventTypeId} for partition {PartitionId} again for the {RetryCount}. time because: \"{FailureReason}\"");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Exception> _handleFilterResult = LoggerMessage
            .Define<Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(1502330189, nameof(HandleFilterResult)),
                "Filter {Filter} in scope {Scope} is handling filtering result for event {EventTypeId} in partition {PartitionId}");

        static readonly Action<ILogger, Guid, Guid, Guid, Guid, Guid, Exception> _filteredEventIsIncluded = LoggerMessage
            .Define<Guid, Guid, Guid, Guid, Guid>(
                LogLevel.Debug,
                new EventId(1603417156, nameof(FilteredEventIsIncluded)),
                "Filter {Filter} in scope {Scope} is writing event {EventTypeId} to partition {PartitionId} in stream {Stream}");

        static readonly Action<ILogger, Guid, Exception> _findingFilterValidator = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(255775676, nameof(FindingFilterValidator)),
                "Finding validator for filter {Filter}");

        static readonly Action<ILogger, Guid, Exception> _validatingFilter = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(1404193471, nameof(ValidatingFilter)),
                "Validating filter {Filter}");

        static readonly Action<ILogger, Type, Type, Exception> _foundValidatorForFilter = LoggerMessage
            .Define<Type, Type>(
                LogLevel.Trace,
                new EventId(362042910, nameof(FoundValidatorForFilter)),
                "Filter definition type {FilterType} can be validated by validator type {ValidatorType}");
        static readonly Action<ILogger, Type, string, Exception> _multipleValidatorsForFilter = LoggerMessage
            .Define<Type, string>(
                LogLevel.Warning,
                new EventId(342051795, nameof(MultipleValidatorsForFilter)),
                "There are multiple validators that can validate filter definition of type {FilterDefinitionType}:\n{ImplementationTypes}\nUsing the first validator");
        
        static readonly Action<ILogger, Guid, Guid, Exception> _validatingFilterForTenant = LoggerMessage
            .Define<Guid, Guid>(
                LogLevel.Debug,
                new EventId(1631987633, nameof(ValidatingFilterForTenant)),
                "Validating filter {Filter} for Tenant {Tenant}");


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

        internal static void ValidatingFilter(this ILogger logger, EventProcessorId filter)
            => _validatingFilter(logger, filter, null);

        internal static void FoundValidatorForFilter(this ILogger logger, Type filterType, Type validatorType)
            => _foundValidatorForFilter(logger, filterType, validatorType, null);

        internal static void MultipleValidatorsForFilter(this ILogger logger, Type filterType, IEnumerable<Type> implementations)
            => _multipleValidatorsForFilter(logger, filterType, string.Join("\n", implementations.Select(_ => _.ToString())), null);

        internal static void ValidatingFilterForTenant(this ILogger logger, EventProcessorId filter, TenantId tenant)
            => _validatingFilterForTenant(logger, filter, tenant, null);
   }
}
