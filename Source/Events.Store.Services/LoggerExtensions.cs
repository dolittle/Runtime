// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.Services;

static class LoggerExtensions
{
    static readonly Action<ILogger, int, string, Exception> _eventsReceivedForCommitting = LoggerMessage
        .Define<int, string>(
            LogLevel.Debug,
            new EventId(2059664795, nameof(EventsReceivedForCommitting)),
            "{NumEvents} {EventType}events received for committing");

    internal static void EventsReceivedForCommitting(this ILogger logger, bool isAggregateEvents, int numEvents)
        => _eventsReceivedForCommitting(logger, numEvents, isAggregateEvents ? "aggregate " : "", null);
}