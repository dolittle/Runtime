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
    /// <summary>
    /// Represents a wrapper of <see cref="IAsyncStreamReader{TClientMessage}"/> that collects the <see cref="ReverseCallArgumentsContext"/> from the first received message and records metrics.
    /// The wrapped stream will also filter out pong messages from the exposed stream, and instead raise events to be handled.
    /// </summary>
    /// <typeparam name="TClientMessage"></typeparam>
    /// <typeparam name="TServerMessage"></typeparam>
    /// <typeparam name="TConnectArguments"></typeparam>
    /// <typeparam name="TConnectResponse"></typeparam>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IAsyncStreamReader<TClientMessage>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        readonly IAsyncStreamReader<TClientMessage> _originalStream;
        readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
        readonly IMetricsCollector _metrics;
        readonly CancellationToken _cancellationToken;
        readonly TaskCompletionSource<ReverseCallArgumentsContext> _reverseCallContext = new(TaskCreationOptions.RunContinuationsAsynchronously);
        bool _firstMoveNextCalled;

        /// <summary>
        /// Initializes a new instance of the <see cref="WrappedAsyncStreamReader{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
        /// </summary>
        /// <param name="originalStream">The original gRPC stream reader to wrap.</param>
        /// <param name="messageConverter">The message converter to use to convert the first message received.</param>
        /// <param name="metrics">The metrics collector to use for metrics about reverse call stream reads.</param>
        /// <param name="cancellationToken">A cancellation token to use for cancelling pending and future reads.</param>
        public WrappedAsyncStreamReader(
            IAsyncStreamReader<TClientMessage> originalStream,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IMetricsCollector metrics,
            CancellationToken cancellationToken)
        {
            _originalStream = originalStream;
            _messageConverter = messageConverter;
            _metrics = metrics;
            _cancellationToken = cancellationToken;
        }


        /// <summary>
        /// Gets a task that resolves when the first message containing the reverse call context is read.
        /// </summary>
        public Task<ReverseCallArgumentsContext> ReverseCallContext => _reverseCallContext.Task;

        /// <summary>
        /// Event that occurs when a message is received from the Client.
        /// </summary>
        public event MessageReceived MessageReceived;

        /// <inheritdoc/>
        public TClientMessage Current => _originalStream.Current;

        /// <inheritdoc/>
        public Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            if (_firstMoveNextCalled)
            {
                return MoveNextSkippingPongsAndRecordMetrics(cancellationToken);
            }

            return FirstMoveNext(cancellationToken);
        }

        async Task<bool> FirstMoveNext(CancellationToken cancellationToken)
        {
            try
            {
                _firstMoveNextCalled = true;
                if (await MoveNextSkippingPongsAndRecordMetrics(cancellationToken).ConfigureAwait(false))
                {
                    SetReverseCallArgumentsContextFromFirstMessage(_originalStream.Current);
                    return true;
                }
                
                SetReverseCallArgumentsContextNotReceived();
                return false;
            }
            catch (TaskCanceledException)
            {
                _reverseCallContext.SetCanceled();
                throw;
            }
            catch (Exception ex)
            {
                _reverseCallContext.SetException(ex);
                throw;
            }
        }

        async Task<bool> MoveNextSkippingPongsAndRecordMetrics(CancellationToken cancellationToken)
        {
            _cancellationToken.ThrowIfCancellationRequested();
            cancellationToken.ThrowIfCancellationRequested();

            using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(_cancellationToken, cancellationToken);
            var combinedCancellationToken = combinedTokenSource.Token;

            while (true)
            {
                if (await _originalStream.MoveNext(combinedCancellationToken).ConfigureAwait(false))
                {
                    _metrics.IncrementTotalStreamReads();
                    _metrics.IncrementTotalStreamReadBytes(_originalStream.Current.CalculateSize());

                    if (CurrentMessageIsPong())
                    {
                        _metrics.IncrementTotalPongsReceived();
                        continue;
                    }

                    MessageReceived?.Invoke();
                    return true;
                }

                return false;
            }
        }

        bool CurrentMessageIsPong()
        {
            return _messageConverter.GetPong(_originalStream.Current) != default;
        }

        void SetReverseCallArgumentsContextFromFirstMessage(TClientMessage message)
        {
            var connectArguments = _messageConverter.GetConnectArguments(message);
            if (connectArguments == default)
            {
                SetReverseCallArgumentsContextNotReceived();
                return;
            }

            var argumentsContext = _messageConverter.GetArgumentsContext(connectArguments);
            if (argumentsContext == default)
            {
                SetReverseCallArgumentsContextNotReceived();
                return;
            }

            _reverseCallContext.SetResult(argumentsContext);
        }

        void SetReverseCallArgumentsContextNotReceived()
        {
            _reverseCallContext.SetException(new ReverseCallArgumentsContextNotReceivedInFirstMessage());
        }
    }
}
