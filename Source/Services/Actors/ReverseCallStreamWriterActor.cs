// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        ILogger logger,
        CancellationToken cancellationToken)
    {
        _requestId = requestId;
        _originalStream = originalStream;
        _metrics = metrics;
        _logger = logger;
        _cancellationToken = cancellationToken;
    }

    public static Props GetProps(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IMetricsCollector metricsCollector,
        ILogger logger,
        CancellationToken cancellationToken) =>
        Props.FromProducer(() => new ReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            requestId,
            originalStream,
            metricsCollector,
            logger,
            cancellationToken));

    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            TServerMessage msg => OnWrite(msg, context),
            Stopping => DisposeAsync().AsTask(),
            _ => Task.CompletedTask
        };

    async Task OnWrite(TServerMessage msg, IContext context)
    {
        if (_cancellationToken.IsCancellationRequested || _disposed)
        {
            context.Respond(Try.Failed(new OperationCanceledException($"{GetType().Name} for request id {_requestId} is cancelled")));
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
            context.Respond(Try.Succeeded());
        }
        catch (Exception e)
        {
            context.Respond(Try.Failed(e));
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
