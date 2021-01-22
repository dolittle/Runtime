// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Logging;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallDispatchers"/>.
    /// </summary>
    public class ReverseCallDispatchers : IReverseCallDispatchers
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerManager _loggerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallDispatchers"/> class.
        /// </summary>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use.</param>
        /// <param name="loggerManager">The <see cref="ILoggerManager"/> to use for creating instances of <see cref="ILogger"/>.</param>
        public ReverseCallDispatchers(IExecutionContextManager executionContextManager, ILoggerManager loggerManager)
        {
            _executionContextManager = executionContextManager;
            _loggerManager = loggerManager;
        }

        /// <inheritdoc/>
        public IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            IAsyncStreamReader<TClientMessage> clientStream,
            IServerStreamWriter<TServerMessage> serverStream,
            ServerCallContext context,
            Func<TClientMessage, TConnectArguments> getConnectArguments,
            Action<TServerMessage, TConnectResponse> setConnectResponse,
            Action<TServerMessage, TRequest> setMessageRequest,
            Func<TClientMessage, TResponse> getMessageResponse,
            Func<TConnectArguments, ReverseCallArgumentsContext> getArgumentsContext,
            Action<TRequest, ReverseCallRequestContext> setRequestContext,
            Func<TResponse, ReverseCallResponseContext> getResponseContex,
            Action<TServerMessage, Ping> setPing,
            Func<TClientMessage, Pong> getPong)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class
            => new ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                clientStream,
                serverStream,
                context,
                getConnectArguments,
                setConnectResponse,
                setMessageRequest,
                getMessageResponse,
                getArgumentsContext,
                setRequestContext,
                getResponseContex,
                setPing,
                getPong,
                _executionContextManager,
                _loggerManager.CreateLogger<ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}