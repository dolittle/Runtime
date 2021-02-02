// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents a extensions for <see cref="ILogger" />.
    /// </summary>
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, Guid, Exception> _handleRequest = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(302266083, nameof(HandleRequest)),
                "Handling request with call id: {CallId}");

        static readonly Action<ILogger, Guid, Exception> _writingResponse = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(369095095, nameof(WritingResponse)),
                "Writing response for request with call id: {CallId}");

        static readonly Action<ILogger, Guid, Exception> _clientCancelled= LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(256383468, nameof(ClientCancelled)),
                "Reverse call client was cancelled before response could be written for request with call id: {CallId}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileWritingResponse = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(1801914240, nameof(ErrorWhileWritingResponse)),
                "Error occurred while writing response for request with call id: {CallId}");

        static readonly Action<ILogger, Guid, Exception> _errorWhileInvokingCallback = LoggerMessage
            .Define<Guid>(
                LogLevel.Warning,
                new EventId(1801914240, nameof(ErrorWhileWritingResponse)),
                "An error occurred while invoking request handler callback for request with call id: {CallId}");

        internal static void HandleRequest(this ILogger logger, ReverseCallId callId)
            => _handleRequest(logger, callId, null);

        internal static void WritingResponse(this ILogger logger, ReverseCallId callId)
            => _writingResponse(logger, callId, null);

        internal static void ErrorWhileInvokingCallback(this ILogger logger, Exception ex, ReverseCallId callId)
            => _errorWhileInvokingCallback(logger, callId, ex);

        internal static void ErrorWhileWritingResponse(this ILogger logger, Exception ex, ReverseCallId callId)
            => _errorWhileWritingResponse(logger, callId, ex);

        internal static void ClientCancelled(this ILogger logger, ReverseCallId callId)
            => _clientCancelled(logger, callId, null);
    }
}
