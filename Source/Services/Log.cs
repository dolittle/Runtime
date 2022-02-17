// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Configuration.ConfigurationObjects.Endpoints;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services;

static partial class Log
{
    [LoggerMessage(0, LogLevel.Warning, "Received arguments, but ping interval was not set")]
    internal static partial void ReceivedArgumentsButPingIntervalNotSet(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Received arguments, but call execution context was not set.")]
    internal static partial void ReceivedArgumentsButCallExecutionContextNotSet(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Received initial message from client, but arguments was not set.")]
    internal static partial void ReceivedInitialMessageByArgumentsNotSet(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Received response.")]
    internal static partial void ReceivedResponse(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Could not find the call id from the received response from the client. The message will be ignored.")]
    internal static partial void CouldNotFindCallId(ILogger logger);

    [LoggerMessage(0, LogLevel.Warning, "Received response from reverse call client, but the call context was not set.")]
    internal static partial void ReceivedResponseButCallContextNotSet(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Received message from reverse call client, but it did not contain a response.")]
    internal static partial void ReceivedMessageButDidNotContainResponse(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "An error occurred during handling of client messages")]
    internal static partial void ErrorWhileHandlingClientMessages(ILogger logger, Exception ex);
    
    [LoggerMessage(0, LogLevel.Debug, "Starting all endpoints")]
    internal static partial void StartingAllEndpoints(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Debug, "Preparing endpoint for {VisibilityType} visibility - running on port {Port}")]
    internal static partial void PreparingEndpoint(ILogger logger, EndpointVisibility visibilityType, int port);
    
    [LoggerMessage(0, LogLevel.Debug, "{VisibilityType} endpoint is disabled")]
    internal static partial void EndpointDisabled(ILogger logger, EndpointVisibility visibilityType);

    [LoggerMessage(0, LogLevel.Debug, "Bind services from {Implementation}")]
    internal static partial void BindServicesFromImplementation(ILogger logger, string implementation);
    
    [LoggerMessage(0, LogLevel.Trace, "Service : {ServiceName}")]
    internal static partial void BoundService(ILogger logger, string serviceName);

    [LoggerMessage(0, LogLevel.Trace, "Waiting for connection arguments")]
    internal static partial void WaitingForConnectionArguments(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Trace, "Received connection arguments")]
    internal static partial void ReceivedConnectionArguments(ILogger logger);
    
    [LoggerMessage(0, LogLevel.Warning, "Connection arguments were not received")]
    internal static partial void ConnectionArgumentsNotReceived(ILogger logger);

    [LoggerMessage(0, LogLevel.Trace, "Connection arguments were not valid")]
    internal static partial void ReceivedInvalidConnectionArguments(ILogger logger);
    
    

    static readonly Action<ILogger, string, Exception> _registeringBoundService = LoggerMessage
        .Define<string>(
            LogLevel.Trace,
            new EventId(943058902, nameof(RegisteringBoundService)),
            "Registering bound service: {ServiceName}");
    
    internal static void RegisteringBoundService(this ILogger logger, string serviceName)
        => _registeringBoundService(logger, serviceName, null);

    static readonly Action<ILogger, string, string, int, Exception> _startingHost = LoggerMessage
        .Define<string, string, int>(
            LogLevel.Information,
            new EventId(322326160, nameof(StartingHost)),
            "Starting {EndpointVisibility} on host: {Host} for port: {Port}");
    
    internal static void StartingHost(this ILogger logger, EndpointVisibility visibility, string host, int port)
        => _startingHost(logger, visibility.ToString(), host, port, null);

    static readonly Action<ILogger, string, Exception> _exposingService = LoggerMessage
        .Define<string>(
            LogLevel.Debug,
            new EventId(259558300, nameof(ExposingService)),
            "Exposing service: {ServiceName}");
    
    internal static void ExposingService(this ILogger logger, string serviceName)
        => _exposingService(logger, serviceName, null);

    static readonly Action<ILogger, string, Exception> _couldNotStartHost = LoggerMessage
        .Define<string>(
            LogLevel.Debug,
            new EventId(170533414, nameof(CouldNotStartHost)),
            "Couldn't start {EndpointVisibility} host");
    
    internal static void CouldNotStartHost(this ILogger logger, Exception ex, EndpointVisibility endpointVisibility)
        => _couldNotStartHost(logger, endpointVisibility.ToString(), ex);

    static readonly Action<ILogger, Guid, Exception> _writingRequest = LoggerMessage
        .Define<Guid>(
            LogLevel.Trace,
            new EventId(382757357, nameof(WritingRequest)),
            "Writing request with call id: {CallId}");
    
    internal static void WritingRequest(this ILogger logger, ReverseCallId callId)
        => _writingRequest(logger, callId, null);

    static readonly Action<ILogger, Exception> _callbackCallFailed = LoggerMessage
        .Define(
            LogLevel.Trace,
            new EventId(221023257, nameof(CallbackCallFailed)),
            "An error occurred while calling a registered callback");
    
    internal static void CallbackCallFailed(this ILogger logger, Exception ex)
        => _callbackCallFailed(logger, ex);

    static readonly Action<ILogger, Exception> _callbackLoopFailed = LoggerMessage
        .Define(
            LogLevel.Trace,
            new EventId(269816071, nameof(CallbackLoopFailed)),
            "An error occurred in the callback loop");
    

    internal static void CallbackLoopFailed(this ILogger logger, Exception ex)
        => _callbackLoopFailed(logger, ex);

    static readonly Action<ILogger, Exception> _callFailed = LoggerMessage
        .Define(
            LogLevel.Warning,
            new EventId(2145278013, nameof(CallFailed)),
            "An error occurred while calling the reverse call dispatcher");
    
    internal static void CallFailed(this ILogger logger, Exception ex)
        => _callFailed(logger, ex);
}
