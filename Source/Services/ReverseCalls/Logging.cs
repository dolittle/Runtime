// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    internal static class Logging
    {
#region WrappedAsyncStreamWriter
        static readonly Action<ILogger, RequestId, Type, Exception> _writingMessage = LoggerMessage.Define<RequestId, Type>(
            LogLevel.Debug,
            new EventId(629413668, nameof(WritingMessage)),
            "Writing message {Type} for {Request}");
        internal static void WritingMessage(this ILogger logger, RequestId requestId, Type messageType)
            => _writingMessage(logger, requestId, messageType, null);

        static readonly Action<ILogger, RequestId, Type, Exception> _writingMessageBlockedByAnotherWrite = LoggerMessage.Define<RequestId, Type>(
            LogLevel.Trace,
            new EventId(825683084, nameof(WritingMessageBlockedByAnotherWrite)),
            "Writing message {Type} for {Request} is blocked by anoter write operation");
        internal static void WritingMessageBlockedByAnotherWrite(this ILogger logger, RequestId requestId, Type messageType)
            => _writingMessageBlockedByAnotherWrite(logger, requestId, messageType, null);

        static readonly Action<ILogger, RequestId, Type, TimeSpan, Exception> _writingMessageUnblockedAfter = LoggerMessage.Define<RequestId, Type, TimeSpan>(
            LogLevel.Trace,
            new EventId(257847899, nameof(WritingMessageUnblockedAfter)),
            "Writing message {Type} for {Request} was unblocked after {BlockedTime}");
        internal static void WritingMessageUnblockedAfter(this ILogger logger, RequestId requestId, Type messageType, TimeSpan blockedTime)
            => _writingMessageUnblockedAfter(logger, requestId, messageType, blockedTime, null);
        
        static readonly Action<ILogger, RequestId, Exception> _writingPing = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(629413668, nameof(WritingPing)),
            "Writing ping for {Request}");
        internal static void WritingPing(this ILogger logger, RequestId requestId)
            => _writingPing(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _writingPingSkipped = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(519331416, nameof(WritingPingSkipped)),
            "Writing ping for {Request} was skipped because anoter write operation is in progress");
        internal static void WritingPingSkipped(this ILogger logger, RequestId requestId)
            => _writingPingSkipped(logger, requestId, null);

        static readonly Action<ILogger, RequestId, TimeSpan, Exception> _wrotePing = LoggerMessage.Define<RequestId, TimeSpan>(
            LogLevel.Trace,
            new EventId(874759618, nameof(WrotePing)),
            "Wrote ping for {Request}, it took {WriteTime}");
        internal static void WrotePing(this ILogger logger, RequestId requestId, TimeSpan writeTime)
            => _wrotePing(logger, requestId, writeTime, null);

        static readonly Action<ILogger, RequestId, TimeSpan, int, Exception> _wroteMessage = LoggerMessage.Define<RequestId, TimeSpan, int>(
            LogLevel.Trace,
            new EventId(485273449, nameof(WroteMessage)),
            "Wrote message for {Request}, it took {WriteTime} and was {MessageSize} bytes");
        internal static void WroteMessage(this ILogger logger, RequestId requestId, TimeSpan writeTime, int messageSize)
            => _wroteMessage(logger, requestId, writeTime, messageSize, null);

        static readonly Action<ILogger, RequestId, Exception> _disposingWrappedAsyncStreamWriter = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(740791129, nameof(DisposingWrappedAsyncStreamWriter)),
            "Disposing of WrappedAsyncStreamWriter for {Request}");
        internal static void DisposingWrappedAsyncStreamWriter(this ILogger logger, RequestId requestId)
            => _disposingWrappedAsyncStreamWriter(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _disposedWrappedAsyncStreamWriter = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(920975132, nameof(DisposedWrappedAsyncStreamWriter)),
            "Disposed of WrappedAsyncStreamWriter for {Request}");
        internal static void DisposedWrappedAsyncStreamWriter(this ILogger logger, RequestId requestId)
            => _disposedWrappedAsyncStreamWriter(logger, requestId, null);
#endregion

#region WrappedAsyncStreamReader
        static readonly Action<ILogger, RequestId, Exception> _readingMessage = LoggerMessage.Define<RequestId>(
            LogLevel.Debug,
            new EventId(001299866, nameof(ReadingMessage)),
            "Reading message for {Request}");
        internal static void ReadingMessage(this ILogger logger, RequestId requestId)
            => _readingMessage(logger, requestId, null);

        static readonly Action<ILogger, RequestId, int, Exception> _readMessage = LoggerMessage.Define<RequestId, int>(
            LogLevel.Trace,
            new EventId(514062882, nameof(ReadMessage)),
            "Read message for for {Request}, it was {MessageSize} bytes");
        internal static void ReadMessage(this ILogger logger, RequestId requestId, int messageSize)
            => _readMessage(logger, requestId, messageSize, null);

        static readonly Action<ILogger, RequestId, Exception> _readPong = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(696612534, nameof(ReadPong)),
            "Read pong for {Request}");
        internal static void ReadPong(this ILogger logger, RequestId requestId)
            => _readPong(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _noMoreMessagesToRead = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(571047019, nameof(NoMoreMessagesToRead)),
            "No more messages to read for {Request}");
        internal static void NoMoreMessagesToRead(this ILogger logger, RequestId requestId)
            => _noMoreMessagesToRead(logger, requestId, null);
#endregion

#region PingedReverseCallConnection

        static readonly Action<ILogger, RequestId, Exception> _waitingForReverseCallContext = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(869827038, nameof(WaitingForReverseCallContext)),
            "Waiting for reverse call arguments context for {Request}");
        internal static void WaitingForReverseCallContext(this ILogger logger, RequestId requestId)
            => _waitingForReverseCallContext(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _receivedReverseCallContext = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(234474181, nameof(ReceivedReverseCallContext)),
            "Received for reverse call arguments context for {Request}");
        internal static void ReceivedReverseCallContext(this ILogger logger, RequestId requestId)
            => _receivedReverseCallContext(logger, requestId, null);

        static readonly Action<ILogger, RequestId, TimeSpan, Exception> _startPings = LoggerMessage.Define<RequestId, TimeSpan>(
            LogLevel.Trace,
            new EventId(722585538, nameof(StartPings)),
            "Starting pings for {Request} every {PingInterval}");
        internal static void StartPings(this ILogger logger, RequestId requestId, TimeSpan pingInterval)
            => _startPings(logger, requestId, pingInterval, null);

        static readonly Action<ILogger, RequestId, TimeSpan, Exception> _startKeepaliveTokenTimeout = LoggerMessage.Define<RequestId, TimeSpan>(
            LogLevel.Trace,
            new EventId(057218397, nameof(StartKeepaliveTokenTimeout)),
            "Starting the keepalive token timeout for {Request} to cancel after {PingInterval} of inactivity");
        internal static void StartKeepaliveTokenTimeout(this ILogger logger, RequestId requestId, TimeSpan pingInterval)
            => _startKeepaliveTokenTimeout(logger, requestId, pingInterval, null);

        static readonly Action<ILogger, RequestId, Exception> _failedToStartPingAndTimeout = LoggerMessage.Define<RequestId>(
            LogLevel.Warning,
            new EventId(670097578, nameof(FailedToStartPingAndTimeout)),
            "Failed to start pinging and keepalive timeout for {Request}, the connection will be cancelled");
        internal static void FailedToStartPingAndTimeout(this ILogger logger, RequestId requestId, Exception exception)
            => _failedToStartPingAndTimeout(logger, requestId, exception);

        static readonly Action<ILogger, RequestId, Exception> _stoppingPings = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(126021885, nameof(StoppingPings)),
            "Stopping pings for {Request}");
        internal static void StoppingPings(this ILogger logger, RequestId requestId)
            => _stoppingPings(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _notStoppingPingsBecauseItWasNotStarted = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(614035582, nameof(NotStoppingPingsBecauseItWasNotStarted)),
            "Not stopping pings for {Request} as they were not started");
        internal static void NotStoppingPingsBecauseItWasNotStarted(this ILogger logger, RequestId requestId)
            => _notStoppingPingsBecauseItWasNotStarted(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _resettingKeepaliveToken = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(251671244, nameof(ResettingKeepaliveToken)),
            "Resetting keepalive token for {Request}");
        internal static void ResettingKeepaliveToken(this ILogger logger, RequestId requestId)
            => _resettingKeepaliveToken(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _keepaliveTimedOut = LoggerMessage.Define<RequestId>(
            LogLevel.Debug,
            new EventId(703967180, nameof(KeepaliveTimedOut)),
            "Keepalive timed out for {Request}");
        internal static void KeepaliveTimedOut(this ILogger logger, RequestId requestId)
            => _keepaliveTimedOut(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _waitingForPingStarterToComplete = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(762374155, nameof(WaitingForPingStarterToComplete)),
            "Waiting for ping starter to complete for {Request}");
        internal static void WaitingForPingStarterToComplete(this ILogger logger, RequestId requestId)
            => _waitingForPingStarterToComplete(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _pingStarterCompleted = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(051164892, nameof(PingStarterCompleted)),
            "Ping starter finished for {Request}");
        internal static void PingStarterCompleted(this ILogger logger, RequestId requestId)
            => _pingStarterCompleted(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _disposingPingedConnection = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(102323333, nameof(DisposingPingedConnection)),
            "Disposing of PingedConnection for {Request}");
        internal static void DisposingPingedConnection(this ILogger logger, RequestId requestId)
            => _disposingPingedConnection(logger, requestId, null);

        static readonly Action<ILogger, RequestId, Exception> _disposedPingedConnection = LoggerMessage.Define<RequestId>(
            LogLevel.Trace,
            new EventId(458986405, nameof(DisposedPingedConnection)),
            "Disposed of PingedConnection for {Request}");
        internal static void DisposedPingedConnection(this ILogger logger, RequestId requestId)
            => _disposedPingedConnection(logger, requestId, null);
#endregion
    }
}
