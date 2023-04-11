// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Actors;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services.ReverseCalls;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using Proto;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Services.Actors;

public class ReverseCallDispatcherActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IActor, IAsyncDisposable
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    public static class Messages
    {
        public record ReceiveArguments(bool NotValidateExecutionContext, CancellationToken CancellationToken);

        public record Accept(TConnectResponse Response, CancellationToken CancellationToken);

        public record Reject(TConnectResponse Response);

        public record Call(TRequest Request, ExecutionContext ExecutionContext);

        public record CallResponse(TResponse Response, ReverseCallId CallId);
    }

    public class Wrapper : IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
    {
        readonly ActorSystem _actorSystem;
        readonly PID _actor;

        public Wrapper(
            ActorSystem actorSystem,
            ICreateProps propsCreator,
            RequestId requestId,
            IPingedConnection<TClientMessage, TServerMessage> connection,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter)
        {
            _actorSystem = actorSystem;
            _actor = actorSystem.Root.SpawnNamed(
                propsCreator.PropsFor<ReverseCallDispatcherActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>>(
                    connection,
                    messageConverter),
                $"reverse-call-dispatcher-{requestId.Value}");
        }

        public void Dispose() => _actor.Stop(_actorSystem);

        public TConnectArguments? Arguments { get; private set; }

        public ExecutionContext? ExecutionContext { get; private set; }

        public async Task<bool> ReceiveArguments(CancellationToken cancellationToken, bool notValidateExecutionContext = false)
        {
            var getResult = await _actorSystem.Root
                .RequestAsync<Try<(bool, TConnectArguments?, ExecutionContext?)>>(
                    _actor,
                    new Messages.ReceiveArguments(notValidateExecutionContext, cancellationToken),
                    CancellationToken.None).ConfigureAwait(false);
            if (!getResult.Success)
            {
                ExceptionDispatchInfo.Capture(getResult.Exception).Throw();
            }

            var (receivedArguments, arguments, executionContext) = getResult.Result;
            Arguments = arguments;
            ExecutionContext = executionContext;
            return receivedArguments;
        }

        public async Task Accept(TConnectResponse response, CancellationToken cancellationToken)
        {
            var request = await _actorSystem.Root.RequestAsync<Try>(_actor, new Messages.Accept(response, cancellationToken), CancellationToken.None)
                .ConfigureAwait(false);
            if (!request.Success)
            {
                ExceptionDispatchInfo.Capture(request.Exception).Throw();
            }
        }

        public async Task Reject(TConnectResponse response, CancellationToken cancellationToken)
        {
            var request = await _actorSystem.Root.RequestAsync<Try>(_actor, new Messages.Reject(response), CancellationToken.None).ConfigureAwait(false);
            if (!request.Success)
            {
                ExceptionDispatchInfo.Capture(request.Exception).Throw();
            }
        }

        public async Task<TResponse> Call(TRequest request, ExecutionContext executionContext, CancellationToken cancellationToken)
        {
            var getResult = await _actorSystem.Root.RequestAsync<Try<TResponse>>(_actor, new Messages.Call(request, executionContext), CancellationToken.None)
                .ConfigureAwait(false);
            if (!getResult.Success)
            {
                ExceptionDispatchInfo.Capture(getResult.Exception).Throw();
            }

            return getResult.Result;
        }
    }

    readonly IPingedConnection<TClientMessage, TServerMessage> _reverseCallConnection;
    readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> _messageConverter;
    readonly ICreateExecutionContexts _executionContextFactory;
    readonly IMetricsCollector _metricsCollector;
    readonly ILogger<ReverseCallDispatcherActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>> _logger;
    readonly Dictionary<ReverseCallId, TaskCompletionSource<TResponse>> _calls = new();

    bool _disposed;
    bool _receivedArguments;
    bool _accepted;
    bool _rejected;

    public ReverseCallDispatcherActor(
        IPingedConnection<TClientMessage, TServerMessage> reverseCallConnection,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        ICreateExecutionContexts executionContextFactory,
        IMetricsCollector metricsCollector,
        ILogger<ReverseCallDispatcherActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>> logger)
    {
        _reverseCallConnection = reverseCallConnection;
        _messageConverter = messageConverter;
        _executionContextFactory = executionContextFactory;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public Task ReceiveAsync(IContext context)
        => context.Message switch
        {
            Messages.ReceiveArguments receiveArguments => OnReceiveArguments(receiveArguments, context),
            Messages.Accept accept => OnAccept(accept, context, context.Respond),
            Messages.Reject reject => OnReject(reject, context.Respond),
            Messages.Call message => OnCall(context, message, context.Respond),
            Messages.CallResponse response => OnCallResponse(response),
            Stopped => DisposeAsync().AsTask(),
            _ => Task.CompletedTask
        };


    Task OnCallResponse(Messages.CallResponse msg)
    {
        if (_calls.Remove(msg.CallId, out var completionSource))
        {
            completionSource.SetResult(msg.Response);
        }
        else
        {
            _logger.CouldNotFindCallId();
        }

        return Task.CompletedTask;
    }

    async Task OnReceiveArguments(Messages.ReceiveArguments msg, IContext context)
    {
        if (_receivedArguments)
        {
            RespondWithError(new ReverseCallDispatcherAlreadyTriedToReceiveArguments());
            return;
        }

        _receivedArguments = true;
        var clientToRuntimeStream = _reverseCallConnection.RuntimeStream;
        if (!await clientToRuntimeStream.MoveNext(msg.CancellationToken).ConfigureAwait(false))
        {
            RespondFailedWithoutError();
            return;
        }

        var arguments = _messageConverter.GetConnectArguments(clientToRuntimeStream.Current);
        if (arguments is null)
        {
            _logger.ReceivedInitialMessageByArgumentsNotSet();
            RespondFailedWithoutError();
            return;
        }

        var callContext = _messageConverter.GetArgumentsContext(arguments);
        if (callContext?.PingInterval == null)
        {
            _logger.ReceivedArgumentsButPingIntervalNotSet();
            RespondFailedWithoutError();
            return;
        }

        if (callContext?.ExecutionContext == null)
        {
            _logger.ReceivedArgumentsButCallExecutionContextNotSet();
            RespondFailedWithoutError();
            return;
        }

        var createExecutionContext = msg.NotValidateExecutionContext
            ? Try<ExecutionContext>.Succeeded(callContext.ExecutionContext.ToExecutionContext())
            : _executionContextFactory.TryCreateUsing(callContext.ExecutionContext);
        if (!createExecutionContext.Success)
        {
            _logger.ReceivedInvalidExecutionContext(createExecutionContext.Exception);
            RespondFailedWithoutError();
            return;
        }

        RespondSucceeded(arguments, createExecutionContext.Result);

        void RespondWithError(Exception ex)
        {
            context.Respond(Try<(bool, TConnectArguments?, ExecutionContext?)>.Failed(ex));
        }

        void RespondFailedWithoutError()
        {
            context.Respond(Try<(bool, TConnectArguments?, ExecutionContext?)>.Succeeded((false, null, null)));
        }

        void RespondSucceeded(TConnectArguments arguments, ExecutionContext executionContext)
        {
            context.Respond(Try<(bool, TConnectArguments?, ExecutionContext?)>.Succeeded((true, arguments, executionContext)));
        }
    }

    async Task OnAccept(Messages.Accept msg, IContext context, Action<Try> respond)
    {
        var hasNotResponded = CheckHasNotResponded();
        if (!hasNotResponded.Success)
        {
            context.Respond(Try.Failed(hasNotResponded.Exception));
            return;
        }

        _accepted = true;
        try
        {
            var message = new TServerMessage();
            _messageConverter.SetConnectResponse(msg.Response, message);
            // Blocking here is okay
            await _reverseCallConnection.ClientStream.WriteAsync(message);
            context.ReenterAfter(ReadClientMessages(context, msg.CancellationToken), async _ =>
            {
                await DisposeAsync().ConfigureAwait(false);
                RespondSuccess();
            });
        }
        catch (Exception e)
        {
            context.Respond(Try.Failed(e));
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

    async Task OnReject(Messages.Reject msg, Action<Try> respond)
    {
        var hasNotResponded = CheckHasNotResponded();
        if (!hasNotResponded.Success)
        {
            respond(Try.Failed(hasNotResponded.Exception));
            return;
        }

        _rejected = true;
        try
        {
            var message = new TServerMessage();
            _messageConverter.SetConnectResponse(msg.Response, message);
            await _reverseCallConnection.ClientStream.WriteAsync(message).ConfigureAwait(false);
            respond(Try.Succeeded);
        }
        catch (Exception e)
        {
            respond(Try.Failed(e));
        }
    }

    Task OnCall(IContext context, Messages.Call msg, Action<Try<TResponse>> respond)
    {
        if (_disposed)
        {
            return RespondError(new CannotPerformCallOnCompletedReverseCallConnection());
        }

        if (_rejected)
        {
            return RespondError(new ReverseCallDispatcherAlreadyRejected());
        }

        var (callId, completionSource) = CreateNewCall();
        var (request, executionContext) = msg;
        context.SafeReenterAfter(
            CallAndWaitForResponse(CreateMessage(callId, request, executionContext), callId, completionSource),
            result =>
            {
                if (!result.Success)
                {
                    _calls.Remove(callId);
                }

                return Respond(result);
            },
            (ex, _) =>
            {
                _calls.Remove(callId);
                return RespondError(ex);
            });
        return Task.CompletedTask;

        Task RespondError(Exception ex)
        {
            respond(Try<TResponse>.Failed(ex));
            return Task.CompletedTask;
        }

        Task Respond(Try<TResponse> result)
        {
            respond(result);
            return Task.CompletedTask;
        }
    }

    async Task<Try<TResponse>> CallAndWaitForResponse(TServerMessage message, ReverseCallId callId, TaskCompletionSource<TResponse> completionSource)
    {
        try
        {
            _logger.WritingRequest(callId);
            _metricsCollector.AddRequest();
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            await _reverseCallConnection.ClientStream.WriteAsync(message).ConfigureAwait(false);
            var response = await completionSource.Task.ConfigureAwait(false);
            stopWatch.Stop();
            _metricsCollector.AddToTotalRequestTime(stopWatch.Elapsed);
            return response;
        }
        catch (Exception e)
        {
            _metricsCollector.AddFailedRequest();
            _logger.CallFailed(e);
            return e;
        }
    }

    (ReverseCallId, TaskCompletionSource<TResponse>) CreateNewCall()
    {
        var completionSource = new TaskCompletionSource<TResponse>(TaskCreationOptions.RunContinuationsAsynchronously);
        var callId = ReverseCallId.New();
        while (!_calls.TryAdd(callId, completionSource))
        {
            callId = ReverseCallId.New();
        }

        return (callId, completionSource);
    }

    TServerMessage CreateMessage(ReverseCallId callId, TRequest request, ExecutionContext executionContext)
    {
        var callContext = new ReverseCallRequestContext
        {
            CallId = callId.ToProtobuf(),
            ExecutionContext = executionContext.ToProtobuf(),
        };

        _messageConverter.SetRequestContext(callContext, request);
        var message = new TServerMessage();
        _messageConverter.SetRequest(request, message);
        return message;
    }

    async Task ReadClientMessages(IContext context, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, _reverseCallConnection.CancellationToken, context.CancellationToken);
        try
        {
            var reader = _reverseCallConnection.RuntimeStream;
            while (!cts.Token.IsCancellationRequested && await reader.MoveNext(cts.Token).ConfigureAwait(false))
            {
                var message = reader.Current;
                var response = _messageConverter.GetResponse(message);
                if (response != null)
                {
                    _logger.ReceivedResponse();
                    var callContext = _messageConverter.GetResponseContext(response);
                    if (callContext?.CallId != null)
                    {
                        ReverseCallId callId = callContext.CallId.ToGuid();
                        context.Send(context.Self, new Messages.CallResponse(response, callId));
                    }
                    else
                    {
                        _logger.ReceivedResponseButCallContextNotSet();
                    }
                }
                else
                {
                    _logger.ReceivedMessageButDidNotContainResponse();
                }
            }
        }
        catch (System.IO.IOException ex) when (ex.Message.StartsWith("The client reset the request stream", StringComparison.Ordinal))
        {
            _logger.ClientDisconnected();
        }
        catch (Exception ex)
        {
            if (!cts.Token.IsCancellationRequested)
            {
                _logger.ErrorWhileHandlingClientMessages(ex);
            }
        }
    }


    public ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return ValueTask.CompletedTask;
        }

        _disposed = true;
        foreach (var (_, completionSource) in _calls)
        {
            try
            {
                completionSource.SetCanceled();
            }
            catch
            {
            }
        }

        _reverseCallConnection?.Dispose();
        return ValueTask.CompletedTask;
    }

    Try CheckHasNotResponded()
    {
        if (_accepted)
        {
            return new ReverseCallDispatcherAlreadyAccepted();
        }

        if (_rejected)
        {
            return new ReverseCallDispatcherAlreadyRejected();
        }

        return Try.Succeeded;
    }
}
