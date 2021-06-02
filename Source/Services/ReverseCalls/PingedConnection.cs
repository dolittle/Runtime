// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Represents an implementation of <see cref="IPingedConnection{TClientMessage, TServerMessage}"/>.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the Runtime to the Client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses sent from the Client to the Runtime.</typeparam>
    public class PingedConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IPingedConnection<TClientMessage, TServerMessage>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly CancellationTokenSource _keepAliveTokenSource;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly RequestId _requestId;
        readonly IMetricsCollector _metrics;
        readonly WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedReader;
        readonly WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedWriter;
        readonly ILogger _logger;
        readonly Task _pingStarter;
        readonly CancellationTokenRegistration  _keepAliveExpiredRegistration;
        TimeSpan _keepaliveTimeout;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PingedConnection{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="requestId">The request id for the gRPC method call.</param>
        /// <param name="requestId">The request id for the gRPC method call.</param>
        /// <param name="runtimeStream">The <see cref="IAsyncStreamReader{TClientMessage}"/> to read messages to the Runtime.</param>
        /// <param name="clientStream">The <see cref="IServerStreamWriter{TServerMessage}"/> to write messages to the Client.</param>
        /// <param name="context">The <see cref="ServerCallContext"/> of the method call.</param>
        /// <param name="messageConverter">The <see cref="MethodConverter"/> to use for decoding the connect arguments and reading the desired ping interval from.</param>
        /// <param name="metrics">The metrics collector to use for metrics about reverse calls.</param>
        /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
        public PingedConnection(
            RequestId requestId,
            IAsyncStreamReader<TClientMessage> runtimeStream,
            IAsyncStreamWriter<TServerMessage> clientStream,
            ServerCallContext context,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IMetricsCollector metrics,
            ILoggerFactory loggerFactory)
        {
            _keepAliveTokenSource = new CancellationTokenSource();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, _keepAliveTokenSource.Token);

            _requestId = requestId;
            _wrappedReader = new(
                requestId,
                runtimeStream,
                messageConverter,
                metrics,
                loggerFactory.CreateLogger<WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                _cancellationTokenSource.Token);
            _wrappedWriter = new(
                requestId,
                clientStream,
                messageConverter,
                metrics,
                loggerFactory.CreateLogger<WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(),
                _cancellationTokenSource.Token);

            _metrics = metrics;
            _logger = loggerFactory.CreateLogger<PingedConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>();

            _pingStarter = WaitForFirstMessageThenStartPinging(_cancellationTokenSource.Token);
            _keepAliveExpiredRegistration = _keepAliveTokenSource.Token.Register(NotifyKeepaliveTimedOut);
        }

        /// <inheritdoc/>
        public IAsyncStreamReader<TClientMessage> RuntimeStream => _wrappedReader;

        /// <inheritdoc/>
        public IAsyncStreamWriter<TServerMessage> ClientStream => _wrappedWriter;

        /// <inheritdoc/>
        public CancellationToken CancellationToken => _cancellationTokenSource.Token;

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;

            if (disposing)
            {
                _logger.DisposingPingedReverseCallConnection(_requestId);
                _cancellationTokenSource.Cancel();
                WaitForPingStarterToCompleteIgnoringExceptions();
                MaybeStopPinging();
                _keepAliveExpiredRegistration.Dispose();
                _wrappedReader.MessageReceived -= ResetKeepaliveTokenCancellation;
                _cancellationTokenSource.Dispose();
                _wrappedWriter.Dispose();
                _logger.DisposedPingedReverseCallConnection(_requestId);
            }

            _disposed = true;
        }

        Task WaitForFirstMessageThenStartPinging(CancellationToken cancellationToken) =>
            Task.Run(async () =>
            {
                var stopwatch = Stopwatch.StartNew();
                _logger.WaitingForReverseCallContext(_requestId);

                var context = await _wrappedReader.ReverseCallContext.ConfigureAwait(false);

                stopwatch.Stop();
                _metrics.AddToTotalWaitForFirstMessageTime(stopwatch.Elapsed);
                _logger.ReceivedReverseCallContext(_requestId);

                var pingInterval = context.PingInterval.ToTimeSpan();
                StartPinging(pingInterval);
                StartKeepaliveTokenTimeout(pingInterval);
            }, cancellationToken);

        void StartPinging(TimeSpan pingInterval)
        {
            _logger.StartPings(_requestId, pingInterval);
            // TODO: to be implemented once the "ping service" is ready
            // _wrappedWriter.MaybeWritePing();
        }

        void MaybeStopPinging()
        {
            // _logger.StoppingPings(_requestId);
            // _logger.NotStoppingPingsBecauseItWasNotStarted(_requestId);
        }

        void StartKeepaliveTokenTimeout(TimeSpan pingInterval)
        {
            _logger.StartKeepaliveTokenTimeout(_requestId, pingInterval);
            _keepaliveTimeout = pingInterval;
            _wrappedReader.MessageReceived += ResetKeepaliveTokenCancellation;
            ResetKeepaliveTokenCancellation();
        }

        void ResetKeepaliveTokenCancellation()
        {
            _cancellationTokenSource.CancelAfter(_keepaliveTimeout);
            _metrics.IncrementTotalKeepaliveTokenResets();
            _logger.ResettingKeepaliveToken(_requestId);
        }

        void NotifyKeepaliveTimedOut()
        {
            _metrics.IncrementTotalKeepaliveTimeouts();
            _logger.KeepaliveTimedOut(_requestId);
        }

        void WaitForPingStarterToCompleteIgnoringExceptions()
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.WaitingForPingStarterToComplete(_requestId);

            _pingStarter.ContinueWith(_ => {}).GetAwaiter().GetResult();

            stopwatch.Stop();
            _metrics.AddToTotalWaitForPingStarterToCompleteTime(stopwatch.Elapsed);
            _logger.PingStarterCompleted(_requestId);
        }
    }
}
