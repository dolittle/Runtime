// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls;

public interface IReverseCallStreamWriterFactory
{
    IReverseCallStreamWriter<TServerMessage> CreatePingedWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class;

    IReverseCallStreamWriter<TServerMessage> CreateWriter<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
        RequestId requestId,
        IAsyncStreamWriter<TServerMessage> originalStream,
        IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class;
}
