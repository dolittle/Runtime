// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Events.Store.Streams
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, ulong, string, Guid, Guid, Exception> _waitForEvent = LoggerMessage
            .Define<ulong, string, Guid, Guid>(
                LogLevel.Trace,
                new EventId(2059664795, nameof(WaitForEvent)),
                "Start waiting for event coming in at position {Position} in {IsPublic}stream {StreamId} in scope {Scope}");

        static readonly Action<ILogger, ulong, string, Guid, Guid, Exception> _waiterNotifyForEvent = LoggerMessage
            .Define<ulong, string, Guid, Guid>(
                LogLevel.Trace,
                new EventId(255784820, nameof(WaiterNotifyForEvent)),
                "Notifying that an event has been written at position {Position} in {IsPublic}stream {StreamId} in scope {Scope}");

        static readonly Action<ILogger, ulong, EventWaiterId, Exception> _waitingTimedOut = LoggerMessage
            .Define<ulong, EventWaiterId>(
                LogLevel.Trace,
                new EventId(188707764, nameof(WaitingTimedOut)),
                "Waiting timedout for position {Position} for {WaiterId}");

        internal static void WaitForEvent(this ILogger logger, StreamPosition position, bool isPublicStream, StreamId stream, ScopeId scope)
            => _waitForEvent(logger, position, isPublicStream ? "public " : string.Empty, stream, scope, null);

        internal static void WaiterNotifyForEvent(this ILogger logger, StreamPosition position, bool isPublicStream, StreamId stream, ScopeId scope)
            => _waiterNotifyForEvent(logger, position, isPublicStream ? "public " : string.Empty, stream, scope, null);

        internal static void WaitingTimedOut(this ILogger logger, StreamPosition position, EventWaiterId eventWaiter)
            => _waitingTimedOut(logger, position, eventWaiter, null);
    }
}