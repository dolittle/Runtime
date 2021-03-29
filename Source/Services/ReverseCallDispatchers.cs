// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
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
        readonly ILoggerFactory _loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallDispatchers"/> class.
        /// </summary>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/>.</param>
        public ReverseCallDispatchers(IExecutionContextManager executionContextManager, ILoggerFactory loggerFactory)
        {
            _executionContextManager = executionContextManager;
            _loggerFactory = loggerFactory;
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
                clientStream,
                serverStream,
                context,
                messageConverter,
                _executionContextManager,
                _loggerFactory.CreateLogger<ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}