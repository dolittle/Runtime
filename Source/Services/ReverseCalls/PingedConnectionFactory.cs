// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Services.Callbacks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls;

/// <summary>
/// Represents an implementation of <see cref="IKeepConnectionsAlive"/>.
/// </summary>
public class PingedConnectionFactory : IKeepConnectionsAlive
{
    readonly ICallbackScheduler _callbackScheduler;
    readonly IMetricsCollector _metricsCollector;
    readonly ILoggerFactory _loggerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="PingedConnectionFactory"/> class.
    /// </summary>
    /// <param name="callbackScheduler">The callback scheduler to use for scheduling pings.</param>
    /// <param name="metricsCollector">The metrics collector to use for metrics about pinged reverse call connections.</param>
    /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
    public PingedConnectionFactory(ICallbackScheduler callbackScheduler, IMetricsCollector metricsCollector, ILoggerFactory loggerFactory)
    {
        _callbackScheduler = callbackScheduler;
        _metricsCollector = metricsCollector;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IPingedConnection<TClientMessage, TServerMessage> CreatePingedReverseCallConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        RequestId requestId,
        IAsyncStreamReader<TClientMessage> runtimeStream,
        IAsyncStreamWriter<TServerMessage> clientStream,
        ServerCallContext context,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
        => new PingedConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            requestId,
            runtimeStream,
            clientStream,
            context,
            messageConverter,
            new TokenSourceDeadline(),
            _callbackScheduler,
            _metricsCollector,
            _loggerFactory);
}