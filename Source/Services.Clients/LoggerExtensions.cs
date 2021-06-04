// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Clients
{
    internal static class LoggerExtensions
    {
#region ReverseCallClient

        static readonly Action<ILogger, Exception> _startingConnection = LoggerMessage.Define(
            LogLevel.Debug,
            new EventId(514365852, nameof(StartingConnection)),
            "Starting  connection");
        internal static void StartingConnection(this ILogger logger)
            => _startingConnection(logger, null);

        static readonly Action<ILogger, Type, TimeSpan, Exception> _sendingConnectArguments = LoggerMessage.Define<Type, TimeSpan>(
            LogLevel.Trace,
            new EventId(266142310, nameof(SendingConnectArguments)),
            "Sending connect arguments of {Type} with ping interval {PingInterval}");
        internal static void SendingConnectArguments(this ILogger logger, Type argumentsType, TimeSpan pingInterval)
            => _sendingConnectArguments(logger, argumentsType, pingInterval, null);

        static readonly Action<ILogger, Type, Exception> _receivingConnectResponse = LoggerMessage.Define<Type>(
            LogLevel.Trace,
            new EventId(681356521, nameof(ReceivingConnectResponse)),
            "Receiving connect response of {Type}");
        internal static void ReceivingConnectResponse(this ILogger logger, Type responseType)
            => _receivingConnectResponse(logger, responseType, null);

        static readonly Action<ILogger, Type, TimeSpan, Exception> _receivedConnectResponse = LoggerMessage.Define<Type, TimeSpan>(
            LogLevel.Trace,
            new EventId(481219694, nameof(ReceivedConnectResponse)),
            "Received connect response of {Type} after {ReceiveTime}");
        internal static void ReceivedConnectResponse(this ILogger logger, Type responseType, TimeSpan receiveTime)
            => _receivedConnectResponse(logger, responseType, receiveTime, null);

        static readonly Action<ILogger, Type, string, Exception> _didNotReceiveConnectResponse = LoggerMessage.Define<Type, string>(
            LogLevel.Warning,
            new EventId(134633945, nameof(DidNotReceiveConnectResponse)),
            "Did not receive connect response of {Type} because: {Reason}");
        internal static void DidNotReceiveConnectResponse(this ILogger logger, Type responseType, string reason)
            => _didNotReceiveConnectResponse(logger, responseType, reason, null);

        static readonly Action<ILogger, Type, Exception> _receivingConnectResponseFailed = LoggerMessage.Define<Type>(
            LogLevel.Warning,
            new EventId(088783007, nameof(ReceivingConnectResponseFailed)),
            "Receiving connect response of {Type} failed");
        internal static void ReceivingConnectResponseFailed(this ILogger logger, Type responseType, Exception exception)
            => _receivingConnectResponseFailed(logger, responseType, exception);

        static readonly Action<ILogger, Type, Exception> _readingMessage = LoggerMessage.Define<Type>(
            LogLevel.Trace,
            new EventId(883108096, nameof(ReadingMessage)),
            "Reading message of {Type}");
        internal static void ReadingMessage(this ILogger logger, Type messageType)
            => _readingMessage(logger, messageType, null);

        static readonly Action<ILogger, Type, int, Exception> _readMessage = LoggerMessage.Define<Type, int>(
            LogLevel.Trace,
            new EventId(561132566, nameof(ReadMessage)),
            "Read message of {Type}, it was {MessageSize} bytes");
        internal static void ReadMessage(this ILogger logger, Type messageType, int messageSize)
            => _readMessage(logger, messageType, messageSize, null);

        static readonly Action<ILogger, Exception> _receivedPing = LoggerMessage.Define(
            LogLevel.Trace,
            new EventId(461238496, nameof(ReceivedPing)),
            "Received ping");
        internal static void ReceivedPing(this ILogger logger)
            => _receivedPing(logger, null);

        static readonly Action<ILogger, Exception> _skippedWritingPong = LoggerMessage.Define(
            LogLevel.Trace,
            new EventId(359799779, nameof(SkippedWritingPong)),
            "Skipped writing pong because another write operation was already in progress");
        internal static void SkippedWritingPong(this ILogger logger)
            => _skippedWritingPong(logger, null);

        static readonly Action<ILogger, Exception> _writingPong = LoggerMessage.Define(
            LogLevel.Trace,
            new EventId(272274337, nameof(WritingPong)),
            "Writing pong");
        internal static void WritingPong(this ILogger logger)
            => _writingPong(logger, null);

        static readonly Action<ILogger, TimeSpan, Exception> _wrotePong = LoggerMessage.Define<TimeSpan>(
            LogLevel.Trace,
            new EventId(423211010, nameof(WrotePong)),
            "Wrote pong, it took {TimeSpan}");
        internal static void WrotePong(this ILogger logger, TimeSpan writeTime)
            => _wrotePong(logger, writeTime, null);

        static readonly Action<ILogger, Type, Exception> _receivedRequest = LoggerMessage.Define<Type>(
            LogLevel.Trace,
            new EventId(750514231, nameof(ReceivedRequest)),
            "Received request of {Type}");
        internal static void ReceivedRequest(this ILogger logger, Type requestType)
            => _receivedRequest(logger, requestType, null);

        static readonly Action<ILogger, Exception> _receivedEmptyMessage = LoggerMessage.Define(
            LogLevel.Trace,
            new EventId(402642085, nameof(ReceivedEmptyMessage)),
            "Received empty message");
        internal static void ReceivedEmptyMessage(this ILogger logger)
            => _receivedEmptyMessage(logger, null);

        static readonly Action<ILogger, string, Exception> _connectionCancelledWhileHandlingRequests = LoggerMessage.Define<string>(
            LogLevel.Trace,
            new EventId(095383500, nameof(ConnectionCancelledWhileHandlingRequests)),
            "Connection was cancelled while handling requests by {Canceller}");
        internal static void ConnectionCancelledWhileHandlingRequests(this ILogger logger, string by)
            => _connectionCancelledWhileHandlingRequests(logger, by, null);

        static readonly Action<ILogger, Exception> _pingTimedOut = LoggerMessage.Define(
            LogLevel.Warning,
            new EventId(896993175, nameof(PingTimedOut)),
            "Ping timed out");
        internal static void PingTimedOut(this ILogger logger)
            => _pingTimedOut(logger, null);

        static readonly Action<ILogger, Type, ReverseCallId, Exception> _handlingRequest = LoggerMessage.Define<Type, ReverseCallId>(
            LogLevel.Trace,
            new EventId(623661706, nameof(HandlingRequest)),
            "Handling request of {Type} for call {CallId}");
        internal static void HandlingRequest(this ILogger logger, Type requestType, ReverseCallId callId)
            => _handlingRequest(logger, requestType, callId, null);

        static readonly Action<ILogger, Type, ReverseCallId, TimeSpan, Exception> _handledRequest = LoggerMessage.Define<Type, ReverseCallId, TimeSpan>(
            LogLevel.Trace,
            new EventId(192136530, nameof(HandledRequest)),
            "Handled request of {Type} for call {CallId}, it took {HandleTime}");
        internal static void HandledRequest(this ILogger logger, Type requestType, ReverseCallId callId, TimeSpan handleTime)
            => _handledRequest(logger, requestType, callId, handleTime, null);

        static readonly Action<ILogger, Type, Exception> _handlingRequestFailed = LoggerMessage.Define<Type>(
            LogLevel.Warning,
            new EventId(952965623, nameof(HandlingRequestFailed)),
            "Handling request of {Type} failed");
        internal static void HandlingRequestFailed(this ILogger logger, Type requestType, Exception exception)
            => _handlingRequestFailed(logger, requestType, exception);

        static readonly Action<ILogger, Type, ReverseCallId, Exception> _handlingRequestFailedToInvokeCallback = LoggerMessage.Define<Type, ReverseCallId>(
            LogLevel.Warning,
            new EventId(946361388, nameof(HandlingRequestFailedToInvokeCallback)),
            "Handling request of {Type} for call {CallId} failed to invoke callback");
        internal static void HandlingRequestFailedToInvokeCallback(this ILogger logger, Type requestType, ReverseCallId callId, Exception exception)
            => _handlingRequestFailedToInvokeCallback(logger, requestType, callId, exception);

        static readonly Action<ILogger, Type, ReverseCallId, Exception> _handlingRequestFailedToWriteResponse = LoggerMessage.Define<Type, ReverseCallId>(
            LogLevel.Warning,
            new EventId(120870812, nameof(HandlingRequestFailedToWriteResponse)),
            "Handling request of {Type} for call {CallId} failed to write response");
        internal static void HandlingRequestFailedToWriteResponse(this ILogger logger, Type requestType, ReverseCallId callId, Exception exception)
            => _handlingRequestFailedToWriteResponse(logger, requestType, callId, exception);

        static readonly Action<ILogger, Type, Exception> _writingMessage = LoggerMessage.Define<Type>(
            LogLevel.Trace,
            new EventId(195379315, nameof(WritingMessage)),
            "Writing message of {Type}");
        internal static void WritingMessage(this ILogger logger, Type messageType)
            => _writingMessage(logger, messageType, null);

        static readonly Action<ILogger, Type, Exception> _writingMessageBlockedByAnotherWrite = LoggerMessage.Define<Type>(
            LogLevel.Trace,
            new EventId(163225797, nameof(WritingMessageBlockedByAnotherWrite)),
            "Writing message of {Type} is blocked by another write operation");
        internal static void WritingMessageBlockedByAnotherWrite(this ILogger logger, Type messageType)
            => _writingMessageBlockedByAnotherWrite(logger, messageType, null);

        static readonly Action<ILogger, Type, TimeSpan, Exception> _writingMessageUnblockedAfter = LoggerMessage.Define<Type, TimeSpan>(
            LogLevel.Trace,
            new EventId(430831474, nameof(WritingMessageUnblockedAfter)),
            "Writing message of {Type} was unblocked after {WaitTime}");
        internal static void WritingMessageUnblockedAfter(this ILogger logger, Type messageType, TimeSpan waitTime)
            => _writingMessageUnblockedAfter(logger, messageType, waitTime, null);

        static readonly Action<ILogger, Type, TimeSpan, int, Exception> _wroteMessage = LoggerMessage.Define<Type, TimeSpan, int>(
            LogLevel.Trace,
            new EventId(321893109, nameof(WroteMessage)),
            "Wrote message of {Type}, it took {WriteTime} and was {MessageSize} bytes");
        internal static void WroteMessage(this ILogger logger, Type messageType, TimeSpan writeTime, int messageSize)
            => _wroteMessage(logger, messageType, writeTime, messageSize, null);
#endregion
    }
}