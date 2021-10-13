// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, ExecutionContext, Exception> _settingExecutionContext = LoggerMessage
            .Define<ExecutionContext>(
                LogLevel.Trace,
                new EventId(41545433, nameof(SettingExecutionContext)),
                "Setting execution context\n{ExecutionContext}");

        static readonly Action<ILogger, Guid, Exception> _validatingFilter = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(1404193471, nameof(ValidatingFilter)),
                "Validating filter: {Filter}");

        static readonly Action<ILogger, Guid, string, Exception> _filterValidationFailed = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Warning,
                new EventId(409191304, nameof(FilterValidationFailed)),
                "Failed to register filter: {Filter}. Filter validation failed because of: {Reason}");

        static readonly Action<ILogger, Guid, Exception> _persistingStreamDefinition = LoggerMessage
            .Define<Guid>(
                LogLevel.Debug,
                new EventId(421949468, nameof(PersistingStreamDefinition)),
                "Persisting definition for stream: {Stream}");

        internal static void SettingExecutionContext(this ILogger logger, ExecutionContext context)
            => _settingExecutionContext(logger, context, null);

        internal static void ValidatingFilter(this ILogger logger, EventProcessorId filter)
            => _validatingFilter(logger, filter, null);

        internal static void ValidatingFilter(this ILogger logger, StreamId filter)
            => _validatingFilter(logger, filter, null);

        internal static void FilterValidationFailed(this ILogger logger, StreamId filterId, FailedFilterValidationReason reason)
            => _filterValidationFailed(logger, filterId, reason, null);

        internal static void PersistingStreamDefinition(this ILogger logger, StreamId filterId)
            => _persistingStreamDefinition(logger, filterId, null);
    }
}
