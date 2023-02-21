// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Rudimentary;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

public class ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IActor, IAsyncDisposable
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

        public Wrapper(ActorSystem actorSystem, ICreateProps props, RequestId requestId, IAsyncStreamWriter<TServerMessage> originalStream, CancellationToken cancellationToken)
        {
            _actorSystem = actorSystem;
            _originalStream = originalStream;
            _actor = actorSystem.Root.SpawnNamed(
                props.PropsFor<ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(
                    requestId,
                    originalStream,
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
    readonly IAsyncStreamWriter<TServerMessage> _originalStream;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;

    Task? _writeInProcess;
    readonly CancellationToken _cancellationToken;
    bool _disposed;

    public ReverseCallStreamWriterActor(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IMetricsCollector metrics,
        ILogger<ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>> logger,
        CancellationToken cancellationToken)
    {
        _requestId = requestId;
        _originalStream = originalStream;
        _metrics = metrics;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            TServerMessage msg => OnWrite(msg, context.Respond),
            Stopping => DisposeAsync().AsTask(),
            _ => Task.CompletedTask
        };

    async Task OnWrite(TServerMessage msg, Action<Try> respond)
    {
        if (_cancellationToken.IsCancellationRequested || _disposed)
        {
            RespondError(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled"));
            return;
        }
        
        _logger.WritingMessage(_requestId, typeof(TServerMessage));
        var stopwatch = Stopwatch.StartNew();
        try
        {
            _writeInProcess = _originalStream.WriteAsync(msg); 
            await _writeInProcess!.ConfigureAwait(false);
            stopwatch.Stop();
            var messageSize = msg.CalculateSize();
            _metrics.AddToTotalStreamWriteTime(stopwatch.Elapsed);
            _metrics.IncrementTotalStreamWrites();
            _metrics.IncrementTotalStreamWriteBytes(messageSize);
            _logger.WroteMessage(_requestId, stopwatch.Elapsed, messageSize);
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
        }
        finally
        {
            _logger.DisposedWrappedAsyncStreamWriter(_requestId);
        }
    }
}
