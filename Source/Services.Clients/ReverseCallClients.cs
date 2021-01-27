// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallClients" />.
    /// </summary>
    public class ReverseCallClients : IReverseCallClients
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;
        readonly TimeSpan _defaultPingInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClients"/> class.
        /// </summary>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager" />.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager" />.</param>
        public ReverseCallClients(IExecutionContextManager executionContextManager, ILoggerManager loggerManager)
        {
            _executionContextManager = executionContextManager;
            _loggerManager = loggerManager;
        }

        /// <inheritdoc/>
        public IReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            Func<AsyncDuplexStreamingCall<TClientMessage, TServerMessage>> establishConnection,
            Action<TClientMessage, TConnectArguments> setConnectArguments,
            Func<TServerMessage, TConnectResponse> getConnectResponse,
            Func<TServerMessage, TRequest> getMessageRequest,
            Action<TClientMessage, TResponse> setMessageResponse,
            Action<TConnectArguments, ReverseCallArgumentsContext> setArgumentsContext,
            Func<TRequest, ReverseCallRequestContext> getRequestContext,
            Action<TResponse, ReverseCallResponseContext> setResponseContext,
            Func<TServerMessage, Ping> getPing,
            Action<TClientMessage, Pong> setPong,
            TimeSpan pingInterval = default)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class =>
                new ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                    establishConnection,
                    setConnectArguments,
                    getConnectResponse,
                    getMessageRequest,
                    setMessageResponse,
                    setArgumentsContext,
                    getRequestContext,
                    setResponseContext,
                    getPing,
                    setPong,
                    pingInterval == default ? _defaultPingInterval : pingInterval,
                    _executionContextManager,
                    _loggerManager.CreateLogger<ReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}
