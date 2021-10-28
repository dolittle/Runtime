// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Events.Store;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Aggregates.Management
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    static class LoggerExtensions
    {
        static readonly Action<ILogger, Exception> _failure = LoggerMessage
            .Define(
                LogLevel.Warning,
                new EventId(12141183, nameof(Failure)),
                "An error occurred");
        
        static readonly Action<ILogger, Exception> _getAllAggregateRoots = LoggerMessage
            .Define(
                LogLevel.Information,
                new EventId(4145433, nameof(GetAllAggregateRoots)),
                "Getting all Aggregate Roots");
        
        static readonly Action<ILogger, Guid,  Exception> _reprocessAllEvents = LoggerMessage
            .Define<Guid>(
                LogLevel.Information,
                new EventId(423124123, nameof(GetOneAggregateRoot)),
                "Getting an Aggregate Root with id {AggregateRootId}");
        
        static readonly Action<ILogger, Guid, string, Exception> _getEvents = LoggerMessage
            .Define<Guid, string>(
                LogLevel.Information,
                new EventId(9184121, nameof(GetEvents)),
                "Getting events committed for Aggregate with Aggregate Root Id {AggregateRootId} and Event Source Id {EventSourceId}");

        internal static void Failure(this ILogger logger, Exception ex)
            => _failure(logger, ex);

        internal static void GetAllAggregateRoots(this ILogger logger)
            => _getAllAggregateRoots(logger, null);

        internal static void GetOneAggregateRoot(this ILogger logger, ArtifactId aggregateRootId)
            => _reprocessAllEvents(logger, aggregateRootId, null);
        internal static void GetEvents(this ILogger logger, ArtifactId aggregateRootId, EventSourceId eventSource)
            => _getEvents(logger, aggregateRootId, eventSource, null);
    }
}
