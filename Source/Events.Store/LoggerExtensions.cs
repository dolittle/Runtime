// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, string, int, Exception> _eventsReceivedForCommitting = LoggerMessage
            .Define<string, int>(
                LogLevel.Debug,
                new EventId(2059664795, nameof(EventsReceivedForCommitting)),
                "{NumEvents} {EventType}events received for committing");

        internal static void EventsReceivedForCommitting(this ILogger logger, bool isAggregateEvents, int numEvents)
            => _eventsReceivedForCommitting(logger, isAggregateEvents? "aggregate " : "", numEvents, null);
    }
}