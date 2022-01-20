// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IReverseCallClients" />.
/// </summary>
public class ReverseCallClients : IReverseCallClients
{
    readonly IClientManager _clientManager;
    readonly IExecutionContextManager _executionContextManager;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly TimeSpan _defaultPingInterval = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallClients"/> class.
    /// </summary>
    /// <param name="clientManager">The client manager to use for creating gRPC clients.</param>
    /// <param name="executionContextManager">The execution context manager to use for setting the execution context for each request.</param>
    /// <param name="metrics">The metrics collector to use for collecting metrics for reverse call clients.</param>
    /// <param name="loggerFactory">The logger factory to use for creating loggers.</param>
    public ReverseCallClients(
        IClientManager clientManager,
        IExecutionContextManager executionContextManager,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory)
    {
        _clientManager = clientManager;
        _executionContextManager = executionContextManager;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
    }

    public IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
        string host,
        int port,
        TimeSpan pingInterval = default)
        where TClient : ClientBase<TClient>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        var client = _clientManager.Get<TClient>(host, port);

        return new ReverseCallClient<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            protocol,
            client,
            pingInterval == default ? _defaultPingInterval : pingInterval,
            _executionContextManager,
            _metrics,
            _loggerFactory.CreateLogger<ReverseCallClient<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}