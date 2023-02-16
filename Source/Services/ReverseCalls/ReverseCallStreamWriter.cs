// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.DependencyInversion;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;
using Proto;

namespace Dolittle.Runtime.Services.ReverseCalls;

[DisableAutoRegistration]
public class ReverseCallStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> : IReverseCallStreamWriter<TServerMessage>
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    readonly ActorSystem? _actorSystem;
    readonly IAsyncStreamWriter<TServerMessage>? _originalStream;
    readonly IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>? _messageConverter;
    readonly PID? _actor;
    readonly bool? _isPinging;
    readonly WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>? _writer;

    public ReverseCallStreamWriter(WrappedAsyncStreamWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> writer)
    {
        _writer = writer;
    }
    
    public ReverseCallStreamWriter(
        ActorSystem actorSystem,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        PID actor,
        bool isPinging)
    {
        _actorSystem = actorSystem;
        _originalStream = originalStream;
        _messageConverter = messageConverter;
        _actor = actor;
        _isPinging = isPinging;
    }

    public WriteOptions? WriteOptions
    {
        get
        {
            return _writer is not null ? _writer.WriteOptions : _originalStream!.WriteOptions;
        }
        set
        {
            if (_writer is not null)
            {
                _writer.WriteOptions = value!;
            }
            else
            {
                _originalStream!.WriteOptions = value;
            }
        }
    }

    public Task WriteAsync(TServerMessage message)
        => _writer is not null ? _writer.WriteAsync(message) : WriteToActor(message);

    public void MaybeWritePing()
    {
        if (_writer is not null)
        {
            _writer.MaybeWritePing();
        }
        else
        {
            MaybePingActor().GetAwaiter().GetResult();
        }
    }


    public void Dispose()
    {
        if (_writer is not null)
        {
            _writer!.Dispose();
        }
        else
        {
            _actor!.Stop(_actorSystem!);
        }
    }

    async Task WriteToActor(TServerMessage message)
    {
        Try result;
        if (!_isPinging!.Value)
        {
            result = await _actorSystem!.Root.RequestAsync<Try>(_actor!, message, CancellationToken.None).ConfigureAwait(false);
        }
        else
        {
            var write = new PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.Write(message, false);
            result = await _actorSystem!.Root.RequestAsync<Try>(_actor!, write, CancellationToken.None).ConfigureAwait(false);
        }

        if (result.Success)
        {
            return;
        }

        ExceptionDispatchInfo.Capture(result.Exception).Throw();
    }

    async Task MaybePingActor()
    {
        if (!_isPinging!.Value)
        {
            return;
        }

        var message = new TServerMessage();
        _messageConverter!.SetPing(message, new Ping());
        var write = new PingingReverseCallStreamWriterActor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>.Write(message, true);
        var result = await _actorSystem!.Root.RequestAsync<Try>(_actor!, write, CancellationToken.None).ConfigureAwait(false);
        if (result.Success)
        {
            return;
        }

        ExceptionDispatchInfo.Capture(result.Exception).Throw();
    }
}
