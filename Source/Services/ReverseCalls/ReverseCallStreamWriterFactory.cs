// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
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
    readonly IMetricsCollector _metrics;
    readonly ILoggerFactory _loggerFactory;
    readonly ReverseCallsConfiguration _reverseCallsConfig;

    public ReverseCallStreamWriterFactory(
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
                new WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                    requestId,
                    originalStream,
                    messageConverter,
                    _metrics,
                    _loggerFactory.CreateLogger<WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                    cancellationToken));
        }

        var actor = _actorSystem.Root.SpawnNamed(
            PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.GetProps(
                requestId,
                originalStream,
                _metrics,
                _loggerFactory.CreateLogger<PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                cancellationToken),
            GetActorName(requestId));
        return new ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            _actorSystem,
            originalStream,
            messageConverter,
            actor,
            true);
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
                new WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                    requestId,
                    originalStream,
                    messageConverter,
                    _metrics,
                    _loggerFactory.CreateLogger<WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                    cancellationToken));
        }

        var actor = _actorSystem.Root.SpawnNamed(
            ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.GetProps(
                requestId,
                originalStream,
                _metrics,
                _loggerFactory.CreateLogger<ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                cancellationToken),
            GetActorName(requestId));
        return new ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            _actorSystem,
            originalStream,
            messageConverter,
            actor,
            false);
    }
    
    static string GetActorName(RequestId requestId) => $"reverse-call-stream-writer-{requestId.Value}";
}
