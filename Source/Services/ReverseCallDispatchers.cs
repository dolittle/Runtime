// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.Actors;
using Dolittle.Runtime.Services.Configuration;
using Dolittle.Runtime.Services.ReverseCalls;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Proto;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="IReverseCallDispatchers"/>.
/// </summary>
public class ReverseCallDispatchers : IReverseCallDispatchers
{
    readonly ActorSystem _actorSystem;
    readonly ICreateProps _props;
    readonly ReverseCallsConfiguration _reverseCallsConfig;
    readonly IIdentifyRequests _requestIdentifier;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly IMetricsCollector _metricsCollector;
    readonly ILoggerFactory _loggerFactory;
    readonly IKeepConnectionsAlive _pingedConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallDispatchers"/> class.
    /// </summary>
    /// <param name="requestIdentifier">The <see cref="IIdentifyRequests"/> for identifying requests.</param>
    /// <param name="executionContextCreator">The <see cref="ICreateExecutionContexts"/> to use.</param>
    /// <param name="metricsCollector">The <see cref="IMetricsCollector"/>.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/>.</param>
    /// <param name="pingedConnectionFactory">The <see cref="IKeepConnectionsAlive"/> for creating pinged connections.</param>
    public ReverseCallDispatchers(
        ActorSystem actorSystem,
        ICreateProps props,
        IOptions<ReverseCallsConfiguration> reverseCallsConfig,
        IIdentifyRequests requestIdentifier,
        ICreateExecutionContexts executionContextCreator,
        IMetricsCollector metricsCollector,
        ILoggerFactory loggerFactory,
        IKeepConnectionsAlive pingedConnectionFactory)
    {
        _actorSystem = actorSystem;
        _props = props;
        _reverseCallsConfig = reverseCallsConfig.Value;
        _requestIdentifier = requestIdentifier;
        _executionContextCreator = executionContextCreator;
        _metricsCollector = metricsCollector;
        _loggerFactory = loggerFactory;
        _pingedConnectionFactory = pingedConnectionFactory;
    }

    /// <inheritdoc/>
    public IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        IAsyncStreamReader<TClientMessage> clientStream,
        IServerStreamWriter<TServerMessage> serverStream,
        ServerCallContext context,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        var requestId = _requestIdentifier.GetRequestIdFor(context);
        var connection = _pingedConnectionFactory.CreatePingedReverseCallConnection(requestId, clientStream, serverStream, context, messageConverter);
        if (_reverseCallsConfig.UseActors)
        {
            return new ReverseCallDispatcherActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.Wrapper(
                _actorSystem,
                _props,
                requestId,
                connection,
                messageConverter);
        }
        return new ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            connection,
            messageConverter,
            _executionContextCreator,
            _metricsCollector,
            _loggerFactory.CreateLogger<ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}
