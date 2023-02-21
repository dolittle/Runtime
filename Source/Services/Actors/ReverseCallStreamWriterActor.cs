// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

public class ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IActor
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
        readonly PID _actor;

        public Wrapper(
            ActorSystem actorSystem,
            ICreateProps props,
            RequestId requestId,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
            IAsyncStreamWriter<TServerMessage> originalStream,
            CancellationToken cancellationToken,
            TimeSpan? pingTimeout)
        {
            _actorSystem = actorSystem;
            _originalStream = originalStream;
            _actor = actorSystem.Root.SpawnNamed(
                props.PropsFor<ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(
                    requestId,
                    messageConverter,
                    originalStream,
                    pingTimeout ?? TimeSpan.Zero,
                    cancellationToken),
                $"reverse-call-stream-writer-{requestId.Value}");
        }

        public async Task WriteAsync(TServerMessage message)
        {
            var result = await _actorSystem.Root.RequestAsync<Try>(_actor, message, CancellationToken.None);
            if (!result.Success)
            {
                ExceptionDispatchInfo.Capture(result.Exception).Throw();
            }
        }

        public WriteOptions? WriteOptions
        {
            get => _originalStream.WriteOptions;
            set => _originalStream.WriteOptions = value;
        }

        public void MaybeWritePing()
        {
        }

        public void Dispose() => _actorSystem.Root.Stop(_actor);
    }

    readonly RequestId _requestId;
    readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
    readonly IAsyncStreamWriter<TServerMessage> _originalStream;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    readonly CancellationToken _cancellationToken;
    readonly TimeSpan? _pingTimeout;

    public ReverseCallStreamWriterActor(
        RequestId requestId,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IMetricsCollector metrics,
        ILogger<ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>> logger,
        TimeSpan pingTimeout,
        CancellationToken cancellationToken)
    {
        _requestId = requestId;
        _messageConverter = messageConverter;
        _originalStream = originalStream;
        _metrics = metrics;
        _logger = logger;
        _cancellationToken = cancellationToken;
        _pingTimeout = pingTimeout == TimeSpan.Zero ? null : pingTimeout;
    }

    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            Started => OnStarted(context),
            ReceiveTimeout => OnPingTimeout(),
            TServerMessage msg => OnWrite(msg, context.Respond),
            Stopping => OnStopped(),
            _ => Task.CompletedTask
        };

    Task OnStarted(IContext context)
    {
        if (_pingTimeout.HasValue)
        {
            context.SetReceiveTimeout(_pingTimeout.Value);
        }
        return Task.CompletedTask;
    }
    
    Task OnStopped()
    {
        _logger.DisposedWrappedAsyncStreamWriter(_requestId);
        return Task.CompletedTask;
    }
    async Task OnPingTimeout()
    {
        var message = new TServerMessage();
        _messageConverter.SetPing(message, new Ping());
        await WriteMessage(message, false, _ => {}).ConfigureAwait(false);
    }

    async Task OnWrite(TServerMessage msg, Action<Try> respond) => await WriteMessage(msg, false, respond).ConfigureAwait(false); 

    async Task WriteMessage(TServerMessage msg, bool isPing, Action<Try> respond)
    {
        LogWriting(isPing);
        if (_cancellationToken.IsCancellationRequested)
        {
            RespondError(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled"));
            return;
        }
        
        var stopwatch = Stopwatch.StartNew();
        try
        {
            await _originalStream.WriteAsync(msg)!.ConfigureAwait(false);
            stopwatch.Stop();
            var messageSize = msg.CalculateSize();
            _metrics.AddToTotalStreamWriteTime(stopwatch.Elapsed);
            _metrics.IncrementTotalStreamWrites();
            _metrics.IncrementTotalStreamWriteBytes(messageSize);
            LogDoneWriting(isPing, stopwatch.Elapsed, messageSize);
            RespondSuccess();
        }
        catch (Exception e)
        {
            RespondError(e);
        }

        void RespondError(Exception e)
        {
            respond(Try.Failed(e));
        }

        void RespondSuccess()
        {
            respond(Try.Succeeded);
        }
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
}
