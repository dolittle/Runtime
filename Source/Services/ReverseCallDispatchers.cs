// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Services.ReverseCalls;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallDispatchers"/>.
    /// </summary>
    public class ReverseCallDispatchers : IReverseCallDispatchers
    {
        readonly IExecutionContextManager _executionContextManager;
        readonly ILoggerFactory _loggerFactory;
        readonly IMetricsCollector _metricsCollector;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallDispatchers"/> class.
        /// </summary>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use.</param>
        /// <param name="loggerFactory">The <see cref="ILoggerFactory"/> to use for creating instances of <see cref="ILogger"/>.</param>
        /// <param name="metricsCollector"><see cref="IMetricsCollector"/> for collecting metrics.</param>
        public ReverseCallDispatchers(
            IExecutionContextManager executionContextManager,
            ILoggerFactory loggerFactory,
            IMetricsCollector metricsCollector)
        {
            _executionContextManager = executionContextManager;
            _loggerFactory = loggerFactory;
            _metricsCollector = metricsCollector;
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
                new PingedReverseCallConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(clientStream, serverStream, context, messageConverter, _metricsCollector),
                messageConverter,
                _executionContextManager,
                _loggerFactory.CreateLogger<ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>());
    }
}