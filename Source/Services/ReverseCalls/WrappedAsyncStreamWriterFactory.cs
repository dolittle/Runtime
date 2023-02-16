// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Services.Configuration;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

public class WrappedAsyncStreamWriterFactory : IWrappedAsyncStreamWriterFactory
{
    readonly ActorSystem _actorSystem;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly ReverseCallsConfiguration _reverseCallsConfig;

    public WrappedAsyncStreamWriterFactory(
        ActorSystem actorSystem,
        IOptions<ReverseCallsConfiguration> reverseCallsConfig,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory)
    {
        _actorSystem = actorSystem;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
        _reverseCallsConfig = reverseCallsConfig.Value;
    }

    public WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> Create<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
        => new(
            _actorSystem,
            _reverseCallsConfig.UseActors,
            requestId,
            originalStream,
            messageConverter,
            _metrics,
            _loggerFactory.CreateLogger<WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
            cancellationToken);
}
