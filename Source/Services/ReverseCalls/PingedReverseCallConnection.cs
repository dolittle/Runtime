// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    public class PingedReverseCallConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IPingedReverseCallConnection<TClientMessage, TServerMessage>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly CancellationTokenSource _keepAliveTokenSource;
        readonly CancellationTokenSource _cancellationTokenSource;
        readonly WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedReader;
        readonly WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _wrappedWriter;
        readonly CancellationTokenRegistration  _keepAliveTokenRegistration;
        readonly Task _pingStarter;
        TimeSpan _keepaliveTimeout;
        bool _disposed;


        public PingedReverseCallConnection(
            IAsyncStreamReader<TClientMessage> runtimeStream,
            IAsyncStreamWriter<TServerMessage> clientStream,
            ServerCallContext context,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IMetricsCollector metrics)
        {
            _keepAliveTokenSource = new CancellationTokenSource();
            _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken, _keepAliveTokenSource.Token);
            _wrappedReader = new(runtimeStream, messageConverter, metrics, _cancellationTokenSource.Token);
            _wrappedWriter = new(clientStream, messageConverter, metrics, _cancellationTokenSource.Token);
            _pingStarter = WaitForFirstMessageThenStartPinging(_cancellationTokenSource.Token);
            _keepAliveTokenRegistration = _keepAliveTokenSource.Token.Register(NotifyKeepaliveTimedOut);
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
                _cancellationTokenSource.Cancel();
                WaitForPingStarterToCompleteIgnoringExceptions();
                MaybeStopPinging();
                _keepAliveTokenRegistration.Dispose();
                _wrappedReader.MessageReceived -= ResetKeepaliveTokenCancellation;
                _cancellationTokenSource.Dispose();
                _wrappedWriter.Dispose();
            }

            _disposed = true;
        }

        Task WaitForFirstMessageThenStartPinging(CancellationToken cancellationToken) =>
            Task.Run(async () =>
            {
                var context = await _wrappedReader.ReverseCallContext.ConfigureAwait(false);
                StartPinging(context);
                StartKeepaliveTokenTimeout(context);
            }, cancellationToken);

        void StartPinging(ReverseCallArgumentsContext context)
        {
            // TODO: to be implemented once the "ping service" is ready
            // _wrappedWriter.MaybeWritePing();
        }

        void MaybeStopPinging()
        {
        }

        void StartKeepaliveTokenTimeout(ReverseCallArgumentsContext context)
        {
            _keepaliveTimeout = context.PingInterval.ToTimeSpan();
            _wrappedReader.MessageReceived += ResetKeepaliveTokenCancellation;
            ResetKeepaliveTokenCancellation();
        }

        void ResetKeepaliveTokenCancellation()
        {
            _cancellationTokenSource.CancelAfter(_keepaliveTimeout);
        }

        void NotifyKeepaliveTimedOut()
        {
            // TODO: Logger
        }

        void WaitForPingStarterToCompleteIgnoringExceptions() =>
            _pingStarter.ContinueWith(_ => {}).GetAwaiter().GetResult();
    }
}
