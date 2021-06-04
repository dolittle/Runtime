// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using Dolittle.Runtime.Services.Callbacks;
using Dolittle.Services.Contracts;
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
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly RequestId _requestId;
        readonly ICancelTokenIfDeadlineIsMissed _keepalive;
        readonly ICallbackScheduler _pingScheduler;
        readonly IMetricsCollector _metrics;
        readonly WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedReader;
        readonly WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedWriter;
        readonly ILogger _logger;
        readonly CancellationTokenRegistration _keepAliveExpiredRegistration;
        Stopwatch _waitForCallContextStopwatch;
        TimeSpan _keepaliveTimeout;
        IDisposable _scheduledPings;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="PingedConnection{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="requestId">The request id for the gRPC method call.</param>
        /// <param name="runtimeStream">The <see cref="IAsyncStreamReader{TClientMessage}"/> to read messages to the Runtime.</param>
        /// <param name="clientStream">The <see cref="IServerStreamWriter{TServerMessage}"/> to write messages to the Client.</param>
        /// <param name="context">The <see cref="ServerCallContext"/> of the method call.</param>
        /// <param name="messageConverter">The <see cref="MethodConverter"/> to use for decoding the connect arguments and reading the desired ping interval from.</param>
        /// <param name="keepalive">The keepalive token canceller to use for keeping track of ping timeouts.</param>
        /// <param name="pingScheduler">The callback scheduler to use for scheduling accurate pings.</param>
        /// <param name="metrics">The metrics collector to use for metrics about reverse calls.</param>
        /// <param name="loggerFactory">The logger factory to use to create loggers.</param>
        public PingedConnection(
            RequestId requestId,
            IAsyncStreamReader<TClientMessage> runtimeStream,
            IAsyncStreamWriter<TServerMessage> clientStream,
            ServerCallContext context,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            ICancelTokenIfDeadlineIsMissed keepalive,
            ICallbackScheduler pingScheduler,
            IMetricsCollector metrics,
            ILoggerFactory loggerFactory)
        {
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, keepalive.Token);

            _requestId = requestId;
            _keepalive = keepalive;
            _pingScheduler = pingScheduler;

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

            WaitForCallContextInFirstMessageThenStartPinging();
            _keepAliveExpiredRegistration = keepalive.Token.Register(NotifyKeepaliveTimedOut);
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
                _logger.DisposingPingedConnection(_requestId);
                _cancellationTokenSource.Cancel();
                MaybeStopPinging();
                _keepAliveExpiredRegistration.Dispose();
                _keepalive.Dispose();
                _wrappedReader.ReverseCallContextReceived -= OnReverseCallContextReceived;
                _wrappedReader.ReverseCallContextNotReceivedInFirstMessage -= OnReverseCallContextNotReceivedInFirstMessage;
                _wrappedReader.MessageReceived -= ResetKeepaliveTokenCancellation;
                _cancellationTokenSource.Dispose();
                _wrappedWriter.Dispose();
                _logger.DisposedPingedConnection(_requestId);
            }

            _disposed = true;
        }

        void WaitForCallContextInFirstMessageThenStartPinging()
        {
            _wrappedReader.ReverseCallContextReceived += OnReverseCallContextReceived;
            _wrappedReader.ReverseCallContextNotReceivedInFirstMessage += OnReverseCallContextNotReceivedInFirstMessage;
            _waitForCallContextStopwatch = Stopwatch.StartNew();
            _logger.WaitingForReverseCallContext(_requestId);
        }

        void OnReverseCallContextReceived(ReverseCallArgumentsContext context)
        {
            _waitForCallContextStopwatch.Stop();
            _metrics.AddToTotalWaitForFirstMessageTime(_waitForCallContextStopwatch.Elapsed);
            _logger.ReceivedReverseCallContext(_requestId, _waitForCallContextStopwatch.Elapsed);

            var pingInterval = context.PingInterval.ToTimeSpan();
            StartPinging(pingInterval);
            StartKeepaliveTokenTimeout(pingInterval);
        }
        void OnReverseCallContextNotReceivedInFirstMessage()
        {
            _waitForCallContextStopwatch.Stop();
            _logger.FailedToStartPingAndTimeout(_requestId, _waitForCallContextStopwatch.Elapsed);
        }

        void StartPinging(TimeSpan pingInterval)
        {
            _logger.StartPings(_requestId, pingInterval);
            _scheduledPings = _pingScheduler.ScheduleCallback(_wrappedWriter.MaybeWritePing, pingInterval);
        }

        void MaybeStopPinging()
        {
            if (_scheduledPings != null)
            {
                _logger.StoppingPings(_requestId);
                _scheduledPings.Dispose();
            }
            else
            {
                _logger.NotStoppingPingsBecauseItWasNotStarted(_requestId);
            }
        }

        void StartKeepaliveTokenTimeout(TimeSpan pingInterval)
        {
            _logger.StartKeepaliveTokenTimeout(_requestId, pingInterval);
            _keepaliveTimeout = pingInterval.Multiply(3);
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
    }
}
