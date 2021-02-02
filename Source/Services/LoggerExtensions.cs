// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    internal static class LoggerExtensions
    {
        static readonly Action<ILogger, string, Exception> _registeringBoundService = LoggerMessage
            .Define<string>(
                LogLevel.Trace,
                new EventId(943058902, nameof(RegisteringBoundService)),
                "Registering bound service: {ServiceName}");

        static readonly Action<ILogger, string, string, int, Exception> _startingHost = LoggerMessage
            .Define<string, string, int>(
                LogLevel.Information,
                new EventId(322326160, nameof(StartingHost)),
                "Starting {EndpointVisibility} on host: {Host} for port: {Port}");

        static readonly Action<ILogger, string, Exception> _exposingService = LoggerMessage
            .Define<string>(
                LogLevel.Debug,
                new EventId(259558300, nameof(ExposingService)),
                "Exposing service: {ServiceName}");
        
        static readonly Action<ILogger, string, Exception> _couldNotStartHost = LoggerMessage
            .Define<string>(
                LogLevel.Debug,
                new EventId(170533414, nameof(CouldNotStartHost)),
                "Couldn't start {EndpointVisibility} host");

        static readonly Action<ILogger, Guid, Exception> _writingRequest = LoggerMessage
            .Define<Guid>(
                LogLevel.Trace,
                new EventId(382757357, nameof(WritingRequest)),
                "Writing request with call id: {CallId}");

        internal static void RegisteringBoundService(this ILogger logger, string serviceName)
            => _registeringBoundService(logger, serviceName, null);

        internal static void StartingHost(this ILogger logger, EndpointVisibility visibility, string host, int port)
            => _startingHost(logger, visibility.ToString(), host, port, null);

        internal static void ExposingService(this ILogger logger, string serviceName)
            => _exposingService(logger, serviceName, null);

        internal static void CouldNotStartHost(this ILogger logger, Exception ex, EndpointVisibility endpointVisibility)
            => _couldNotStartHost(logger, endpointVisibility.ToString(), ex);

        internal static void WritingRequest(this ILogger logger, ReverseCallId callId)
            => _writingRequest(logger, callId, null);
    }
}