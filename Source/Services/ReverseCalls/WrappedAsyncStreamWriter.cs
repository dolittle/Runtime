// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Represents a thread-safe wrapper of <see cref="IAsyncStreamWriter{TServerMessage}"/> that also exposes special handling of ping messages and records metrics.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the Runtime to the Client.</typeparam>
    /// <typeparam name="TResponse">Type of the responses sent from the Client to the Runtime.</typeparam>
    public class WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IAsyncStreamWriter<TServerMessage>, IDisposable
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly RequestId _requestId;
        readonly IAsyncStreamWriter<TServerMessage> _originalStream;
        readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
        readonly IMetricsCollector _metrics;
        readonly ILogger _logger;
        readonly CancellationToken _cancellationToken;
        readonly SemaphoreSlim _writeLock = new(1);
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedAsyncStreamWriter{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="requestId">The request id for the gRPC method call.</param>
        /// <param name="originalStream">The original gRPC stream writer to wrap.</param>
        /// <param name="messageConverter">The message converter to use to create ping messages.</param>
        /// <param name="metrics">The metrics collector to use for metrics about reverse call stream writes.</param>
        /// <param name="logger">The logger to use.</param>
        /// <param name="cancellationToken">A cancellation token to use for cancelling pending and future writes.</param>
        public WrappedAsyncStreamWriter(
            RequestId requestId,
            IAsyncStreamWriter<TServerMessage> originalStream,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IMetricsCollector metrics,
            ILogger logger,
            CancellationToken cancellationToken)
        {
            _requestId = requestId;
            _originalStream = originalStream;
            _messageConverter = messageConverter;
            _metrics = metrics;
            _logger = logger;
            _cancellationToken = cancellationToken;
        }

        /// <inheritdoc/>
        public WriteOptions WriteOptions
        {
            get => _originalStream.WriteOptions;
            set => _originalStream.WriteOptions = value;
        }

        /// <summary>
        /// Writes a message asynchronously, blocking until previous writes have been completed.
        /// </summary>
        /// <param name="message">The message to be written. Cannot be null.</param>
        /// <returns>A task that will complete when the message is sent.</returns>
        public async Task WriteAsync(TServerMessage message)
        {
            _logger.WritingMessage(_requestId, typeof(TServerMessage));
            _cancellationToken.ThrowIfCancellationRequested();

            if (!_writeLock.Wait(0))
            {
                var stopwatch = Stopwatch.StartNew();

                _metrics.IncrementPendingStreamWrites();
                _logger.WritingMessageBlockedByAnotherWrite(_requestId, typeof(TServerMessage));

                await _writeLock.WaitAsync(_cancellationToken).ConfigureAwait(false);

                stopwatch.Stop();
                _metrics.DecrementPendingStreamWrites();
                _metrics.AddToTotalStreamWriteWaitTime(stopwatch.Elapsed);
                _logger.WritingMessageUnblockedAfter(_requestId, typeof(TServerMessage), stopwatch.Elapsed);
            }

            await WriteRecordMetricsAndReleaseLock(message, false);
        }

        /// <summary>
        /// Writes a ping message asynchronously if another write operation is not in progress.
        /// </summary>
        /// <returns>A task that will complete when the ping is sent or skipped.</returns>
        public Task MaybeWritePing()
        {
            _logger.WritingPing(_requestId);
            _cancellationToken.ThrowIfCancellationRequested();

            if (!_writeLock.Wait(0))
            {
                _logger.WritingPingSkipped(_requestId);
                return Task.CompletedTask;
            }

            var ping = new Ping();
            var message = new TServerMessage();
            _messageConverter.SetPing(message, ping);

            return WriteRecordMetricsAndReleaseLock(message, true);
        }

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
                _logger.DisposingWrappedAsyncStreamWriter(_requestId);
                _writeLock.Wait();
                _writeLock.Dispose();
                _logger.DisposedWrappedAsyncStreamWriter(_requestId);
            }

            _disposed = true;
        }

        async Task WriteRecordMetricsAndReleaseLock(TServerMessage message, bool messageIsPing)
        {
            try
            {
                _cancellationToken.ThrowIfCancellationRequested();

                var stopwatch = Stopwatch.StartNew();

                await _originalStream.WriteAsync(message).ConfigureAwait(false);

                stopwatch.Stop();
                var messageSize = message.CalculateSize();
                _metrics.AddToTotalStreamWriteTime(stopwatch.Elapsed);
                _metrics.IncrementTotalStreamWrites();
                _metrics.IncrementTotalStreamWriteBytes(messageSize);

                if (messageIsPing)
                {
                    _metrics.IncrementTotalPingsSent();
                    _logger.WrotePing(_requestId, stopwatch.Elapsed);
                }
                else
                {
                    _logger.WroteMessage(_requestId, stopwatch.Elapsed, messageSize);
                }
            }
            finally
            {
                _writeLock.Release();
            }
        }
    }
}