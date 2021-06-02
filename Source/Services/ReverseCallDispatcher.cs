// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Services.ReverseCalls;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Represents an implementation of <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public class ReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        : IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly SemaphoreSlim _writeSemaphore = new(1);
        readonly ConcurrentDictionary<ReverseCallId, TaskCompletionSource<TResponse>> _calls = new();
        readonly IPingedReverseCallConnection<TClientMessage, TServerMessage> _reverseCallConnection;
        readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
        readonly IExecutionContextManager _executionContextManager;
        readonly ILogger _logger;

        readonly object _receiveArgumentsLock = new();
        readonly object _respondLock = new();
        bool _completed;
        bool _disposed;

        bool _receivingArguments;
        bool _accepted;
        bool _rejected;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="reverseCallConnection">The <see cref="IPingedReverseCallConnection{TClientMessage, TServerMessage}"/></param>
        /// <param name="messageConverter">The <see cref="IConvertReverseCallMessages{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="executionContextManager">The <see cref="IExecutionContextManager"/> to use.</param>
        /// <param name="logger">The <see cref="ILogger"/> to use.</param>
        public ReverseCallDispatcher(
            IPingedReverseCallConnection<TClientMessage, TServerMessage> reverseCallConnection,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IExecutionContextManager executionContextManager,
            ILogger logger)
        {
            _reverseCallConnection = reverseCallConnection;
            _messageConverter = messageConverter;
            _executionContextManager = executionContextManager;
            _logger = logger;
        }

        /// <inheritdoc/>
        public TConnectArguments Arguments { get; private set; }

        /// <inheritdoc/>
        public async Task<bool> ReceiveArguments(CancellationToken cancellationToken)
        {
            ThrowIfReceivingArguments();
            lock (_receiveArgumentsLock)
            {
                ThrowIfReceivingArguments();
                _receivingArguments = true;
            }

            var clientToRuntimeStream = _reverseCallConnection.RuntimeStream;
            if (await clientToRuntimeStream.MoveNext(cancellationToken).ConfigureAwait(false))
            {
                var arguments = _messageConverter.GetConnectArguments(clientToRuntimeStream.Current);
                if (arguments != null)
                {
                    var callContext = _messageConverter.GetArgumentsContext(arguments);
                    if (callContext?.PingInterval == null)
                    {
                        _logger.LogWarning("Received arguments, but ping interval was not set");
                        return false;
                    }

                    if (callContext?.ExecutionContext != null)
                    {
                        _executionContextManager.CurrentFor(callContext.ExecutionContext.ToExecutionContext());
                        Arguments = arguments;
                        return true;
                    }

                    _logger.LogWarning("Received arguments, but call execution context was not set.");
                }
                else
                {
                    _logger.LogWarning("Received initial message from client, but arguments was not set.");
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public async Task Accept(TConnectResponse response, CancellationToken cancellationToken)
        {
            ThrowIfResponded();
            lock (_respondLock)
            {
                ThrowIfResponded();
                _accepted = true;
            }

            var message = new TServerMessage();
            _messageConverter.SetConnectResponse(response, message);
            await _reverseCallConnection.ClientStream.WriteAsync(message).ConfigureAwait(false);
            await HandleClientMessages(cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public Task Reject(TConnectResponse response, CancellationToken cancellationToken)
        {
            ThrowIfResponded();
            lock (_respondLock)
            {
                ThrowIfResponded();
                _rejected = true;
            }

            var message = new TServerMessage();
            _messageConverter.SetConnectResponse(response, message);
            return _reverseCallConnection.ClientStream.WriteAsync(message);
        }

        /// <inheritdoc/>
        public async Task<TResponse> Call(TRequest request, CancellationToken cancellationToken)
        {
            ThrowIfCompletedCall();

            var completionSource = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
            var callId = ReverseCallId.New();
            while (!_calls.TryAdd(callId, completionSource))
            {
                callId = ReverseCallId.New();
            }

            try
            {
                await _writeSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                try
                {
                    var callContext = new ReverseCallRequestContext
                    {
                        CallId = callId.ToProtobuf(),
                        ExecutionContext = _executionContextManager.Current.ToProtobuf(),
                    };
                    _messageConverter.SetRequestContext(callContext, request);

                    var message = new TServerMessage();
                    _messageConverter.SetRequest(request, message);
                    _logger.LogTrace("Writing request with CallId: {CallId}", callId);
                    _logger.WritingRequest(callId);
                    await _reverseCallConnection.ClientStream.WriteAsync(message).ConfigureAwait(false);
                }
                finally
                {
                    _writeSemaphore.Release();
                }

                return await completionSource.Task.ConfigureAwait(false);
            }
            catch
            {
                _calls.TryRemove(callId, out _);
                throw;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose managed and unmanaged resources.
        /// </summary>
        /// <param name="disposing">Whether to dispose managed resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _writeSemaphore.Dispose();
                    _reverseCallConnection.Dispose();
                }

                _disposed = true;
            }
        }

        async Task HandleClientMessages(CancellationToken cancellationToken)
        {
            using var jointCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            try
            {
                var clientToRuntimeStream = _reverseCallConnection.RuntimeStream;
                while (!jointCts.IsCancellationRequested && await clientToRuntimeStream.MoveNext(jointCts.Token).ConfigureAwait(false))
                {
                    var message = clientToRuntimeStream.Current;
                    var response = _messageConverter.GetResponse(clientToRuntimeStream.Current);
                    if (response != null)
                    {
                        _logger.LogTrace("Received response");
                        var callContext = _messageConverter.GetResponseContext(response);
                        if (callContext?.CallId != null)
                        {
                            ReverseCallId callId = callContext.CallId.ToGuid();
                            if (_calls.TryRemove(callId, out var completionSource))
                            {
                                completionSource.SetResult(response);
                            }
                            else
                            {
                                _logger.LogWarning("Could not find the call id from the received response from the client. The message will be ignored.");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Received response from reverse call client, but the call context was not set.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Received message from reverse call client, but it did not contain a response.");
                    }
                }
            }
            catch (Exception ex)
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning(ex, "An error occurred during handling of client messages");
                }
            }
            finally
            {
                _completed = true;
                foreach ((_, var completionSource) in _calls)
                {
                    try
                    {
                        completionSource.SetCanceled();
                    }
                    catch
                    {
                    }
                }
            }
        }

        void ThrowIfReceivingArguments()
        {
            if (_receivingArguments)
            {
                throw new ReverseCallDispatcherAlreadyTriedToReceiveArguments();
            }
        }

        void ThrowIfResponded()
        {
            if (_accepted)
            {
                throw new ReverseCallDispatcherAlreadyAccepted();
            }
            else if (_rejected)
            {
                throw new ReverseCallDispatcherAlreadyRejected();
            }
        }

        void ThrowIfCompletedCall()
        {
            if (_completed)
            {
                throw new CannotPerformCallOnCompletedReverseCallConnection();
            }
        }
    }
}
