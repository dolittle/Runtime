// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.ReverseCalls;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Represents an implementation of <see cref="IReverseCallDispatchers"/>.
/// </summary>
public class ReverseCallDispatchers : IReverseCallDispatchers
{
    readonly IIdentifyRequests _requestIdentifier;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly ILoggerFactory _loggerFactory;
    readonly IKeepConnectionsAlive _pingedConnectionFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallDispatchers"/> class.
    /// </summary>
    /// <param name="requestIdentifier">The <see cref="IIdentifyRequests"/> for identifying requests.</param>
    /// <param name="executionContextCreator">The <see cref="ICreateExecutionContexts"/> to use.</param>
    /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/>.</param>
    /// <param name="pingedConnectionFactory">The <see cref="IKeepConnectionsAlive"/> for creating pinged connections.</param>
    public ReverseCallDispatchers(
        IIdentifyRequests requestIdentifier,
        ICreateExecutionContexts executionContextCreator,
        ILoggerFactory loggerFactory,
        IKeepConnectionsAlive pingedConnectionFactory)
    {
        _requestIdentifier = requestIdentifier;
        _executionContextCreator = executionContextCreator;
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
        => new ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            _pingedConnectionFactory.CreatePingedReverseCallConnection(_requestIdentifier.GetRequestIdFor(context), clientStream, serverStream, context, messageConverter),
            messageConverter,
            _executionContextCreator,
            _loggerFactory.CreateLogger<ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
}
