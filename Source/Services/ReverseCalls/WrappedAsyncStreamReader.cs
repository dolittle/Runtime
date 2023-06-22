// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Dolittle.Runtime.Services.ReverseCalls;

/// <summary>
/// Represents a wrapper of <see cref="IAsyncStreamReader{TClientMessage}"/> that collects the <see cref="ReverseCallArgumentsContext"/> from the first received message and records metrics.
/// The wrapped stream will also filter out pong messages from the exposed stream, and instead raise events to be handled.
/// </summary>
/// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the Runtime to the Client.</typeparam>
/// <typeparam name="TResponse">Type of the responses sent from the Client to the Runtime.</typeparam>
public class WrappedAsyncStreamReader<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IAsyncStreamReader<TClientMessage>
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    readonly RequestId _requestId;
    readonly IAsyncStreamReader<TClientMessage> _originalStream;
    readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    readonly CancellationToken _cancellationToken;
    bool _firstMoveNextCalled;

    /// <summary>
    /// Initializes a new instance of the <see cref="WrappedAsyncStreamReader{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="requestId">The request id for the gRPC method call.</param>
    /// <param name="originalStream">The original gRPC stream reader to wrap.</param>
    /// <param name="messageConverter">The message converter to use to convert the first message received.</param>
    /// <param name="metrics">The metrics collector to use for metrics about reverse call stream reads.</param>
    /// <param name="logger">The logger to use.</param>
    /// <param name="cancellationToken">A cancellation token to use for cancelling pending and future reads.</param>
    public WrappedAsyncStreamReader(
        RequestId requestId,
        IAsyncStreamReader<TClientMessage> originalStream,
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

    /// <summary>
    /// Event that occurs when a message is received from the Client.
    /// </summary>
    public event MessageReceived MessageReceived;

    /// <summary>
    /// Event that occurs when the reverse call context is received from the Client.
    /// </summary>
    public event ReverseCallContextReceived ReverseCallContextReceived;

    /// <summary>
    /// Event that occurs when the reverse call context is not received in the first message from the Client.
    /// </summary>
    public event ReverseCallContextNotReceivedInFirstMessage ReverseCallContextNotReceivedInFirstMessage;

    /// <inheritdoc/>
    public TClientMessage Current => _originalStream.Current;

    /// <inheritdoc/>
    public Task<bool> MoveNext(CancellationToken cancellationToken)
    {
        _logger.ReadingMessage(_requestId);
        return _firstMoveNextCalled
            ? MoveNextSkippingPongsAndRecordMetrics(cancellationToken)
            : FirstMoveNext(cancellationToken);
    }

    async Task<bool> FirstMoveNext(CancellationToken cancellationToken)
    {
        try
        {
            _firstMoveNextCalled = true;
            if (await MoveNextSkippingPongsAndRecordMetrics(cancellationToken).ConfigureAwait(false))
            {
                FetchReverseCallArgumentsContextFromFirstMessage(_originalStream.Current);
                return true;
            }

            ReverseCallArgumentsNotReceivedBecauseNoFirstMessage();
            return false;
        }
        catch
        {
            ReverseCallContextNotReceivedInFirstMessage?.Invoke();
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
                var messageSize = _originalStream.Current.CalculateSize();
                _metrics.IncrementTotalStreamReads();
                _metrics.IncrementTotalStreamReadBytes(messageSize);

                MessageReceived?.Invoke();

                if (CurrentMessageIsPong())
                {
                    _metrics.IncrementTotalPongsReceived();
                    _logger.ReadPong(_requestId);
                    continue;
                }

                _logger.ReadMessage(_requestId, messageSize);
                return true;
            }

            _logger.NoMoreMessagesToRead(_requestId);
            return false;
        }
    }

    bool CurrentMessageIsPong()
        => _messageConverter.GetPong(_originalStream.Current) != default;

    void FetchReverseCallArgumentsContextFromFirstMessage(TClientMessage message)
    {
        var connectArguments = _messageConverter.GetConnectArguments(message);
        if (connectArguments == default)
        {
            ReverseCallArgumentsNotReceivedBecauseNoConnectArgumentsInFirstMessage();
            return;
        }

        var argumentsContext = _messageConverter.GetArgumentsContext(connectArguments);
        if (argumentsContext == default)
        {
            ReverseCallArgumentsNotReceivedBecauseNoContextOnConnectArguments();
            return;
        }

        ReverseCallContextReceived?.Invoke(argumentsContext);
    }

    void ReverseCallArgumentsNotReceivedBecauseNoFirstMessage()
    {
        ReverseCallContextNotReceivedInFirstMessage?.Invoke();
        _logger.ReverseCallArgumentsNotReceivedBecauseNoFirstMessage(_requestId);
    }

    void ReverseCallArgumentsNotReceivedBecauseNoConnectArgumentsInFirstMessage()
    {
        ReverseCallContextNotReceivedInFirstMessage?.Invoke();
        _logger.ReverseCallArgumentsNotReceivedBecauseNoFirstMessage(_requestId);
    }

    void ReverseCallArgumentsNotReceivedBecauseNoContextOnConnectArguments()
    {
        ReverseCallContextNotReceivedInFirstMessage?.Invoke();
        _logger.ReverseCallArgumentsNotReceivedBecauseNoFirstMessage(_requestId);
    }
}
