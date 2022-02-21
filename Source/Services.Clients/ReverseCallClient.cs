// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Represents an implementation of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
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
    /// <summary>
    /// The amount of ping intervals to wait until it times out. 
    /// </summary>
    public static readonly int PingThreshold = 3;

    readonly IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _protocol;
    readonly TClient _client;
    readonly TimeSpan _pingInterval;
    readonly ICreateExecutionContexts _executionContextCreator;
    readonly IMetricsCollector _metrics;
    readonly ILogger _logger;
    readonly SemaphoreSlim _writeLock = new(1);
    readonly object _acceptHandleLock = new();
    IClientStreamWriter<TClientMessage> _clientToServer;
    IAsyncStreamReader<TServerMessage> _serverToClient;
    ReverseCallHandler<TRequest, TResponse> _callback;
    bool _connecting;
    bool _connectionEstablished;
    bool _startedHandling;
    bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallClient{TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="protocol">The protocol for this reverse call.</param>
    /// <param name="client">The client to use to start the reverse call.</param>
    /// <param name="pingInterval">The interval to request and expect pings to keep the connection alive.</param>
    /// <param name="executionContextCreator">The execution context creator to use for validating incoming execution contexts.</param>
    /// <param name="metrics">The metrics to use for reporting metrics.</param>
    /// <param name="logger">The logger to use for logging.</param>
    public ReverseCallClient(
        IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
        TClient client,
        TimeSpan pingInterval,
        ICreateExecutionContexts executionContextCreator,
        IMetricsCollector metrics,
        ILogger logger)
    {
        ThrowIfInvalidPingInterval(pingInterval);
        _protocol = protocol;
        _client = client;
        _pingInterval = pingInterval;
        _executionContextCreator = executionContextCreator;
        _metrics = metrics;
        _logger = logger;
    }

    /// <inheritdoc/>
    public TConnectResponse ConnectResponse { get; private set; }

    /// <inheritdoc/>
    public async Task<bool> Connect(TConnectArguments connectArguments, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        EnsureOnlyConnectingOnce();

        StartConnection(cancellationToken);

        await SendConnectArguments(connectArguments, executionContext, cancellationToken).ConfigureAwait(false);

        return await ReceiveConnectResponse(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task Handle(ReverseCallHandler<TRequest, TResponse> callback, CancellationToken cancellationToken)
    {
        ThrowIfConnectionNotEstablished();
        EnsureOnlyHandlingOnce();

        _callback = callback;

        using var keepalive = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        try
        {
            keepalive.CancelAfter(_pingInterval.Multiply(PingThreshold));

            while (await ReadMessageFromServerWhileRespondingToPings(keepalive).ConfigureAwait(false))
            {
                var request = _protocol.GetRequest(_serverToClient.Current);
                if (request != default)
                {
                    _metrics.IncrementTotalRequestsReceived();
                    _logger.ReceivedRequest(typeof(TRequest));
                    StartRequestHandler(request, cancellationToken);
                }
                else
                {
                    _metrics.IncrementTotalEmptyMessagesReceived();
                    _logger.ReceivedEmptyMessage();
                }

                keepalive.CancelAfter(_pingInterval.Multiply(PingThreshold));
            }
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _metrics.IncrementTotalCancelledConnections();
                _logger.ConnectionCancelledWhileHandlingRequests("client");
                return;
            }

            if (!keepalive.IsCancellationRequested)
            {
                _metrics.IncrementTotalCancelledConnections();
                _logger.ConnectionCancelledWhileHandlingRequests("server");
                return;
            }

            _metrics.IncrementTotalPingTimeouts();
            _logger.PingTimedOut();

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
                _writeLock.Dispose();
            }

            _disposed = true;
        }
    }

    void StartConnection(CancellationToken cancellationToken)
    {
        _logger.StartingConnection();
        _metrics.IncrementTotalStartedConnections();

        var connection = _protocol.Call(_client, new CallOptions(cancellationToken: cancellationToken));
        _clientToServer = connection.RequestStream;
        _serverToClient = connection.ResponseStream;
    }

    Task SendConnectArguments(TConnectArguments connectArguments, ExecutionContext executionContext, CancellationToken cancellationToken)
    {
        _logger.SendingConnectArguments(typeof(TConnectArguments), _pingInterval);

        var callContext = new ReverseCallArgumentsContext
        {
            ExecutionContext = executionContext.ToProtobuf(),
            PingInterval = Duration.FromTimeSpan(_pingInterval)
        };
        _protocol.SetConnectArgumentsContext(callContext, connectArguments);
        var message = new TClientMessage();
        _protocol.SetConnectArguments(connectArguments, message);

        return WriteMessage(message, cancellationToken);
    }

    async Task<bool> ReceiveConnectResponse(CancellationToken cancellationToken)
    {
        using var keepalive = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        try
        {
            keepalive.CancelAfter(_pingInterval.Multiply(PingThreshold));

            var stopwatch = Stopwatch.StartNew();
            _logger.ReceivingConnectResponse(typeof(TConnectResponse));

            if (await ReadMessageFromServerWhileRespondingToPings(keepalive).ConfigureAwait(false))
            {
                var response = _protocol.GetConnectResponse(_serverToClient.Current);
                if (response == default)
                {
                    _logger.DidNotReceiveConnectResponse(typeof(TConnectResponse), "Message did not contain a connect response");
                    return false;
                }

                stopwatch.Stop();
                _logger.ReceivedConnectResponse(typeof(TConnectResponse), stopwatch.Elapsed);
                _metrics.AddToTotalWaitForConnectResponseTime(stopwatch.Elapsed);

                ConnectResponse = response;
                _connectionEstablished = true;
                return true;
            }

            _logger.DidNotReceiveConnectResponse(typeof(TConnectResponse), "No message received from server");
            return false;
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.Cancelled)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _metrics.IncrementTotalCancelledConnections();
                _logger.DidNotReceiveConnectResponse(typeof(TConnectResponse), "Connection was closed by the client");
                return false;
            }

            if (!keepalive.IsCancellationRequested)
            {
                _metrics.IncrementTotalCancelledConnections();
                _logger.DidNotReceiveConnectResponse(typeof(TConnectResponse), "Connection was closed by the server");
                return false;
            }

            _metrics.IncrementTotalPingTimeouts();
            _logger.PingTimedOut();

            throw new PingTimedOut();
        }
        catch (Exception exception)
        {
            _logger.ReceivingConnectResponseFailed(typeof(TConnectResponse), exception);
            throw;
        }
    }

    void StartRequestHandler(TRequest request, CancellationToken cancellationToken)
        => Task.Run(() => OnReceivedRequest(request, cancellationToken), cancellationToken);

    async Task<bool> ReadMessageFromServerWhileRespondingToPings(CancellationTokenSource keepalive)
    {
        while (!keepalive.Token.IsCancellationRequested)
        {
            _logger.ReadingMessage(typeof(TClientMessage));

            if (await _serverToClient.MoveNext(keepalive.Token).ConfigureAwait(false))
            {
                var message = _serverToClient.Current;
                var messageSize = message.CalculateSize();
                _logger.ReadMessage(typeof(TClientMessage), messageSize);
                _metrics.IncrementTotalReceivedMessages();
                _metrics.AddToTotalReceivedBytes(messageSize);

                if (MessageIsPing(message))
                {
                    _logger.ReceivedPing();
                    _metrics.IncrementTotalPingsReceived();

                    await MaybeRespondWithPong().ConfigureAwait(false);

                    keepalive.CancelAfter(_pingInterval.Multiply(PingThreshold));
                    continue;
                }

                return true;
            }

            return false;
        }

        return false;
    }

    bool MessageIsPing(TServerMessage message)
        => _protocol.GetPing(message) != default;

    async Task MaybeRespondWithPong()
    {
        if (!_writeLock.Wait(0))
        {
            _logger.SkippedWritingPong();
            return;
        }
        try
        {
            var stopwatch = Stopwatch.StartNew();

            _logger.WritingPong();

            var message = new TClientMessage();
            _protocol.SetPong(new Pong(), message);
            await _clientToServer.WriteAsync(message).ConfigureAwait(false);

            stopwatch.Stop();
            var messageSize = message.CalculateSize();
            _metrics.AddToTotalWriteTime(stopwatch.Elapsed);
            _metrics.IncrementTotalWrites();
            _metrics.AddToTotalWriteBytes(messageSize);
            _metrics.IncrementTotalPongsSent();
            _logger.WrotePong(stopwatch.Elapsed);
        }
        finally
        {
            _writeLock.Release();
        }
    }


    async Task OnReceivedRequest(TRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var requestContext = _protocol.GetRequestContext(request);
            ReverseCallId callId = requestContext.CallId.ToGuid();

            try
            {
                _logger.HandlingRequest(typeof(TRequest), callId);
                var stopwatch = Stopwatch.StartNew();

                // TODO: Is this even going to work? This should mainly be used for the Event Horizon, not sure what microservice and tenant is set in there?
                var createExecutionContext = _executionContextCreator.TryCreateUsing(requestContext.ExecutionContext);
                if (!createExecutionContext.Success)
                {
                    throw createExecutionContext.Exception;
                }
                
                var response = await _callback(request, createExecutionContext.Result, cancellationToken).ConfigureAwait(false);

                stopwatch.Stop();
                _metrics.AddToTotalRequestHandlingTime(stopwatch.Elapsed);
                _logger.HandledRequest(typeof(TRequest), callId, stopwatch.Elapsed);

                await WriteResponse(response, callId, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                _metrics.IncrementTotalFailedRequestCallbacks();
                _logger.HandlingRequestFailedToInvokeCallback(typeof(TRequest), callId, exception);
            }
        }
        catch (Exception exception)
        {
            _metrics.IncrementTotalFailedRequestHandlers();
            _logger.HandlingRequestFailed(typeof(TRequest), exception);
        }
    }

    async Task WriteResponse(TResponse response, ReverseCallId callId, CancellationToken cancellationToken)
    {
        try
        {
            var responseContext = new ReverseCallResponseContext { CallId = callId.ToProtobuf() };
            _protocol.SetResponseContext(responseContext, response);
            var message = new TClientMessage();
            _protocol.SetResponse(response, message);

            await WriteMessage(message, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            _metrics.IncrementTotalFailedResponseWrites();
            _logger.HandlingRequestFailedToWriteResponse(typeof(TRequest), callId, exception);
        }
    }

    async Task WriteMessage(TClientMessage message, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _logger.WritingMessage(typeof(TClientMessage));

        if (!_writeLock.Wait(0))
        {
            var stopwatch = Stopwatch.StartNew();

            _metrics.IncrementPendingWrites();
            _logger.WritingMessageBlockedByAnotherWrite(typeof(TClientMessage));

            await _writeLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                stopwatch.Stop();
                _metrics.DecrementPendingWrites();
                _metrics.AddToTotalWriteWaitTime(stopwatch.Elapsed);
                _logger.WritingMessageUnblockedAfter(typeof(TServerMessage), stopwatch.Elapsed);
            }
            catch
            { }
        }

        try
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stopwatch = Stopwatch.StartNew();

            await _clientToServer.WriteAsync(message).ConfigureAwait(false);

            stopwatch.Stop();
            var messageSize = message.CalculateSize();
            _metrics.AddToTotalWriteTime(stopwatch.Elapsed);
            _metrics.IncrementTotalWrites();
            _metrics.AddToTotalWriteBytes(messageSize);
            _logger.WroteMessage(typeof(TClientMessage), stopwatch.Elapsed, messageSize);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    void ThrowIfInvalidPingInterval(TimeSpan pingInterval)
    {
        if (pingInterval.TotalMilliseconds <= 0)
        {
            throw new PingIntervalNotGreaterThanZero();
        }
    }

    void EnsureOnlyConnectingOnce()
    {

        ThrowIfAlreadyConnecting();
        lock (_acceptHandleLock)
        {
            ThrowIfAlreadyConnecting();
            _connecting = true;
        }
    }

    void ThrowIfAlreadyConnecting()
    {
        if (_connecting)
        {
            throw new ReverseCallClientAlreadyCalledConnect();
        }
    }

    void ThrowIfConnectionNotEstablished()
    {
        if (!_connectionEstablished)
        {
            throw new ReverseCallClientNotConnected();
        }
    }

    void EnsureOnlyHandlingOnce()
    {

        ThrowIfAlreadyStartedHandling();
        lock (_acceptHandleLock)
        {
            ThrowIfAlreadyStartedHandling();
            _startedHandling = true;
        }
    }

    void ThrowIfAlreadyStartedHandling()
    {
        if (_startedHandling)
        {
            throw new ReverseCallClientAlreadyStartedHandling();
        }
    }
}
