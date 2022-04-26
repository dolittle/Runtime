// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Debug, "Events were successfully committed")]
    internal static partial void EventsSuccessfullyCommitted(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Error committing events")]
    internal static partial void ErrorCommittingEvents(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Debug, "Aggregate events were successfully committed")]
    internal static partial void AggregateEventsSuccessfullyCommitted(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Error committing aggregate events")]
    internal static partial void ErrorCommittingAggregateEvents(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Debug, "Fetch events for aggregate")]
    internal static partial void FetchEventsForAggregate(this ILogger logger);

    [LoggerMessage(0, LogLevel.Debug, "Successfully fetched events for aggregate")]
    internal static partial void SuccessfullyFetchedEventsForAggregate(this ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Error fetching events from aggregate")]
    internal static partial void ErrorFetchingEventsFromAggregate(this ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Warning, "Error fetching catchup events")]
    internal static partial void ErrorFetchingCatchupEvents(this ILogger logger, Exception ex);
    
    static readonly Action<ILogger, int, string, Exception> _eventsReceivedForCommitting = LoggerMessage
        .Define<int, string>(
            LogLevel.Debug,
            new EventId(2059664795, nameof(EventsReceivedForCommitting)),
            "{NumEvents} {EventType}events received for committing");

    internal static void EventsReceivedForCommitting(this ILogger logger, bool isAggregateEvents, int numEvents)
        => _eventsReceivedForCommitting(logger, numEvents, isAggregateEvents ? "aggregate " : "", null);
}
