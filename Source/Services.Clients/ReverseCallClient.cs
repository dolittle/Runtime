// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Microsoft.Extensions.Logging;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TClient">Type of the client to use for calls to the server.</typeparam>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public class ReverseCallClient<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse>, IDisposable
        where TClient : ClientBase<TClient>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _protocol;
        readonly TClient _client;
        readonly TimeSpan _pingInterval;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;
        readonly SemaphoreSlim _writeResponseSemaphore = new(1);
        readonly object _connectLock = new();
        readonly object _handleLock = new();
        IClientStreamWriter<TClientMessage> _clientToServer;
        IAsyncStreamReader<TServerMessage> _serverToClient;
        bool _connecting;
        bool _connectionEstablished;
        bool _startedHandling;
        bool _disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="protocol">The protocol for this reverse call.</param>
        /// <param name="pingInterval">The interval to request and expect pings to keep the connection alive.</param>
        /// <param name="executionContextManager">The execution context manager to use for setting the execution context for each request.</param>
        /// <param name="logger">The logger to use.</param>
        public ReverseCallClient(
            IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
            TClient client,
            TimeSpan pingInterval,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            ThrowIfInvalidPingInterval(pingInterval);
            _protocol = protocol;
            _client = client;
            _pingInterval = pingInterval;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public TConnectResponse ConnectResponse { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> Connect(TConnectArguments connectArguments, CancellationToken cancellationToken)
        {
            ThrowIfConnecting();
            lock (_connectLock)
            {
                ThrowIfConnecting();
                _connecting = true;
            }

            var callOptions = new CallOptions(cancellationToken: cancellationToken);
            var streamingCall = _protocol.Call(_client, callOptions);
            _clientToServer = streamingCall.RequestStream;
            _serverToClient = streamingCall.ResponseStream;
            var callContext = new ReverseCallArgumentsContext
            {
                ExecutionContext = _executionContextManager.Current.ToProtobuf(),
                PingInterval = Duration.FromTimeSpan(_pingInterval)
            };
            _protocol.SetConnectArgumentsContext(callContext, connectArguments);
            var message = new TClientMessage();
            _protocol.SetConnectArguments(connectArguments, message);

            await _clientToServer.WriteAsync(message).ConfigureAwait(false);

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(_pingInterval.Multiply(3));

            try
            {
                if (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
                {
                    var response = _protocol.GetConnectResponse(_serverToClient.Current);
                    if (response != null)
                    {
                        _logger.LogTrace("Received connect response");
                        ConnectResponse = response;
                        _connectionEstablished = true;
                        return true;
                    }
                    else
                    {
                        _logger.LogWarning("Did not receive connect response. Server message did not contain the connect response");
                    }
                }
                else
                {
                    _logger.LogWarning("Did not receive connect response. Server stream was empty");
                }

                await _clientToServer.CompleteAsync().ConfigureAwait(false);
                return false;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled by client while connecting");
                }
                else
                {
                    _logger.LogWarning("Reverse Call Client was cancelled by server while connecting");
                }

                return false;
            }
        }

        /// <inheritdoc/>
        public async Task Handle(Func<TRequest, CancellationToken, Task<TResponse>> callback, CancellationToken cancellationToken)
        {
            ThrowIfConnectionNotEstablished();
            ThrowIfAlreadyStartedHandling();
            lock (_handleLock)
            {
                ThrowIfAlreadyStartedHandling();
                _startedHandling = true;
            }

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            linkedCts.CancelAfter(_pingInterval.Multiply(3));
            try
            {
                while (await _serverToClient.MoveNext(linkedCts.Token).ConfigureAwait(false))
                {
                    var message = _serverToClient.Current;
                    var ping = _protocol.GetPing(message);
                    var request = _protocol.GetRequest(message);
                    if (ping != null)
                    {
                        _logger.LogTrace("Received ping");
                        await WritePong(cancellationToken).ConfigureAwait(false);
                    }
                    else if (request != null)
                    {
                        _ = Task.Run(() => OnReceivedRequest(callback, request, cancellationToken));
                    }
                    else
                    {
                        _logger.LogWarning("Received message from Reverse Call Dispatcher, but it was not a request or a ping");
                    }

                    linkedCts.CancelAfter(_pingInterval.Multiply(3));
                }
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled by client while handling requests");
                    return;
                }

                if (!linkedCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Reverse Call Client was cancelled by server while handling requests");
                    return;
                }

                throw new PingTimedOut();
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose the managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writeResponseSemaphore.Dispose();
                }

                _disposed = true;
            }
        }

        async Task WritePong(CancellationToken cancellationToken)
        {
            await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogDebug("Reverse Call Client was cancelled before it could respond with pong");
                    return;
                }

                var message = new TClientMessage();
                _protocol.SetPong(new Pong(), message);

                _logger.LogTrace("Writing pong");
                await _clientToServer.WriteAsync(message).ConfigureAwait(false);
            }
            finally
            {
                _writeResponseSemaphore.Release();
            }
        }

        async Task OnReceivedRequest(
            Func<TRequest, CancellationToken, Task<TResponse>> callback,
            TRequest request,
            CancellationToken cancellationToken)
        {
            try
            {
                var requestContext = _protocol.GetRequestContext(request);
                ReverseCallId callId = requestContext.CallId.ToGuid();
                TResponse response;
                try
                {
                    _logger.LogTrace("Handling request with call '{CallId}'", callId);
                    _logger.HandleRequest(callId);
                    _executionContextManager.CurrentFor(requestContext.ExecutionContext);
                    response = await callback(request, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.ErrorWhileInvokingCallback(ex, callId);
                    return;
                }

                await _writeResponseSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    await WriteResponse(response, callId, cancellationToken).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.ErrorWhileWritingResponse(ex, callId);
                }
                finally
                {
                    _writeResponseSemaphore.Release();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "An error occurred while handling received request");
            }
        }

        Task WriteResponse(TResponse response, ReverseCallId callId, CancellationToken cancellationToken)
        {
            var responseContext = new ReverseCallResponseContext { CallId = callId.ToProtobuf() };
            _protocol.SetResponseContext(responseContext, response);
            var message = new TClientMessage();
            _protocol.SetResponse(response, message);
            if (!cancellationToken.IsCancellationRequested)
            {
                _logger.WritingResponse(callId);
                return _clientToServer.WriteAsync(message);
            }

            _logger.ClientCancelled(callId);
            return Task.CompletedTask;
        }

        void ThrowIfInvalidPingInterval(TimeSpan pingInterval)
        {
            if (pingInterval.TotalMilliseconds <= 0) throw new PingIntervalNotGreaterThanZero();
        }

        void ThrowIfConnecting()
        {
            if (_connecting)
            {
                throw new ReverseCallClientAlreadyCalledConnect();
            }
        }

        void ThrowIfAlreadyStartedHandling()
        {
            if (_startedHandling)
            {
                throw new ReverseCallClientAlreadyStartedHandling();
            }
        }

        void ThrowIfConnectionNotEstablished()
        {
            if (!_connectionEstablished)
            {
                throw new ReverseCallClientNotConnected();
            }
        }
    }
}
