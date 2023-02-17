// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.DependencyInversion.Lifecycle;
using Dolittle.Runtime.Services.Configuration;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

[Singleton]
public class ReverseCallStreamWriterFactory : IReverseCallStreamWriterFactory
{
    readonly ActorSystem _actorSystem;
    readonly ICreateProps _props;
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly ReverseCallsConfiguration _reverseCallsConfig;

    public ReverseCallStreamWriterFactory(
        ActorSystem actorSystem,
        ICreateProps props,
        IOptions<ReverseCallsConfiguration> reverseCallsConfig,
        IMetricsCollector metrics,
        ILoggerFactory loggerFactory)
    {
        _actorSystem = actorSystem;
        _props = props;
        _metrics = metrics;
        _loggerFactory = loggerFactory;
        _reverseCallsConfig = reverseCallsConfig.Value;
    }
    
    public IReverseCallStreamWriter<TServerMessage> CreatePingedWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
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
    {
        if (!_reverseCallsConfig.UseActors)
        {
            return new ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                    requestId,
                    originalStream,
                    messageConverter,
                    _metrics,
                    _loggerFactory.CreateLogger<ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                    cancellationToken);
        }
        return new PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.Wrapper(
            _actorSystem,
            _props,
            requestId,
            originalStream,
            messageConverter,
            cancellationToken);
    }

    public IReverseCallStreamWriter<TServerMessage> CreateWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
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
    {
        if (!_reverseCallsConfig.UseActors)
        {
            return new ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                    requestId,
                    originalStream,
                    messageConverter,
                    _metrics,
                    _loggerFactory.CreateLogger<ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                    cancellationToken);
        }
        return new ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.Wrapper(
            _actorSystem,
            _props,
            requestId,
            originalStream,
            cancellationToken);
    }
}
