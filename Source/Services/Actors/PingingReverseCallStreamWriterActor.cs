// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

public class PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IActor, IAsyncDisposable
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    
    public class Wrapper : IReverseCallStreamWriter<TServerMessage>
    {
        readonly ActorSystem _actorSystem;
        readonly IAsyncStreamWriter<TServerMessage> _originalStream;
        readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
        readonly PID _actor;

        public Wrapper(
            ActorSystem actorSystem,
            ICreateProps props,
            RequestId requestId,
            IAsyncStreamWriter<TServerMessage> originalStream,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            CancellationToken cancellationToken)
        {
            _actorSystem = actorSystem;
            _originalStream = originalStream;
            _messageConverter = messageConverter;
            _actor = actorSystem.Root.SpawnNamed(
                props.PropsFor<PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(
                    requestId,
                    originalStream,
                    cancellationToken),
                $"pinging-reverse-call-stream-writer-{requestId.Value}");
        }

        public Task WriteAsync(TServerMessage message) => SendWrite(message, false);

        public WriteOptions? WriteOptions
        {
            get => _originalStream.WriteOptions;
            set => _originalStream.WriteOptions = value;
        }


        public void MaybeWritePing()
        {
            var message = new TServerMessage();
            _messageConverter.SetPing(message, new Ping());
            SendWrite(message, true).GetAwaiter().GetResult();
        }

        async Task SendWrite(TServerMessage message, bool isPing)
        {
            var result = await _actorSystem.Root.RequestAsync<Try>(_actor, new Write(message, isPing), CancellationToken.None);
            if (!result.Success)
            {
                ExceptionDispatchInfo.Capture(result.Exception).Throw();
            }
        }

        public void Dispose() => _actorSystem.Root.Stop(_actor);
    }
    
    readonly RequestId _requestId;
    readonly IAsyncStreamWriter<TServerMessage> _originalStream;
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

    public PingingReverseCallStreamWriterActor(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IMetricsCollector metrics,
        ILogger<PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>> logger,
        CancellationToken cancellationToken)
    {
        _requestId = requestId;
        _originalStream = originalStream;
        _metrics = metrics;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public record Write(TServerMessage Message, bool IsPing);

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
                context.Respond(Try.Succeeded);
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
            context.Send(sender, completedTask.IsCompletedSuccessfully
                ? Try.Succeeded
                : Try.Failed(completedTask.Exception?.InnerException ?? completedTask.Exception!));
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
        }
        finally
        {
            await foreach (var (_, (messageEnvelope, context)) in _bufferedMessages.Reader.ReadAllAsync(CancellationToken.None))
            {
                context.Send(messageEnvelope.Sender!, Try.Failed(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled")));
            }
            _logger.DisposedWrappedAsyncStreamWriter(_requestId);
        }
    }
}
