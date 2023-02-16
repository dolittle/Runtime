// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

/// <summary>
/// Represents a thread-safe wrapper of <see cref="IAsyncStreamWriter{TServerMessage}"/> that also exposes special handling of ping messages and records metrics.
/// </summary>
/// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the Runtime to the Client.</typeparam>
/// <typeparam name="TResponse">Type of the responses sent from the Client to the Runtime.</typeparam>
public class ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IActor, IAsyncDisposable
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
    readonly Channel<(TServerMessage, CapturedContext)> _bufferedMessages = Channel.CreateUnbounded<(TServerMessage, CapturedContext)>(new UnboundedChannelOptions
    {
        SingleReader = true,
        SingleWriter = true
    });

    readonly CancellationToken _cancellationToken;
    
    Task? _writeInProcess;
    bool _isWriting;
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
    public ReverseCallStreamWriterActor(
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

    public record Write(TServerMessage Message, bool IsPing);
    public static Props GetProps(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        IMetricsCollector metricsCollector,
        ILogger logger,
        CancellationToken cancellationToken) =>
            Props.FromProducer(() => new ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                requestId,
                originalStream,
                messageConverter,
                metricsCollector,
                logger,
                cancellationToken));

    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            Write msg => OnWrite(msg, context),
            Stopping => DisposeAsync().AsTask(),
            _ => Task.CompletedTask
        };

    Task OnWrite(Write msg, IContext context)
    {
        if (_cancellationToken.IsCancellationRequested || _disposed)
        {
            context.Respond(Try.Failed(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled")));
            return Task.CompletedTask;
        }

        if (_isWriting)
        {
            if (msg.IsPing)
            {
                context.Respond(Try.Succeeded());
                return Task.CompletedTask;
            }
            _bufferedMessages.Writer.TryWrite((msg.Message, context.Capture()));
            return Task.CompletedTask;
        }

        return WriteMessage(msg.Message, msg.IsPing, context, context.Sender!);
    }

    Task WriteMessage(TServerMessage msg, bool isPing, IContext context, PID sender)
    {
        LogWriting(isPing);
        if (_cancellationToken.IsCancellationRequested || _disposed)
        {
            context.Send(sender, Try.Failed(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled")));
            return Task.CompletedTask;
        }

        _isWriting = true;
        var stopwatch = Stopwatch.StartNew();
        _writeInProcess = _originalStream.WriteAsync(msg);
        context.ReenterAfter(_writeInProcess, completedTask =>
        {
            _isWriting = false;
            stopwatch.Stop();
            if (completedTask.IsCompletedSuccessfully)
            {
                var messageSize = msg.CalculateSize();
                _metrics.AddToTotalStreamWriteTime(stopwatch.Elapsed);
                _metrics.IncrementTotalStreamWrites();
                _metrics.IncrementTotalStreamWriteBytes(messageSize);
                LogDoneWriting(isPing, stopwatch.Elapsed, messageSize);
            }
            context.Send(sender, completedTask.IsCompletedSuccessfully ? Try.Succeeded() : Try.Failed(completedTask.Exception?.InnerException ?? completedTask.Exception!));
            return _bufferedMessages.Reader.TryRead(out var item)
                ? WriteMessage(item.Item1, false, item.Item2.Context, item.Item2.MessageEnvelope.Sender!)
                : Task.CompletedTask;
        });
        return Task.CompletedTask;
    }

    void LogWriting(bool isPing)
    {
        if (isPing)
        {
            _logger.WritingPing(_requestId);
        }
        else
        {
            _logger.WritingMessage(_requestId, typeof(TServerMessage));
        }
    }

    void LogDoneWriting(bool isPing, TimeSpan writeTime, int messageSize)
    {
        if (isPing)
        {
            _metrics.IncrementTotalPingsSent();
            _logger.WrotePing(_requestId, writeTime);
        }
        else
        {
            _logger.WroteMessage(_requestId, writeTime, messageSize);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }
        try
        {
            _disposed = true;
            _logger.DisposingWrappedAsyncStreamWriter(_requestId);
            var writeInProcess = _writeInProcess ?? Task.CompletedTask;
            await writeInProcess.ConfigureAwait(false);
            _bufferedMessages.Writer.Complete();
            await foreach (var (_, (messageEnvelope, context)) in _bufferedMessages.Reader.ReadAllAsync(CancellationToken.None))
            {
                context.Send(messageEnvelope.Sender!, Try.Failed(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled")));
            }
        }
        finally
        {
            _logger.DisposedWrappedAsyncStreamWriter(_requestId);
        }
    }
}
