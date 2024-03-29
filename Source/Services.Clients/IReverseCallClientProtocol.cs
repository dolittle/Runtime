// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines the protocol for reverse call service clients.
/// </summary>
/// <typeparam name="TClient">Type of the client to use for calls to the server.</typeparam>
/// <typeparam name="TClientMessage">Type of the messages that is sent from the client to the server.</typeparam>
/// <typeparam name="TServerMessage">Type of the messages that is sent from the server to the client.</typeparam>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
public interface IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> :
    ICanCallADuplexStreamingMethod<TClient, TClientMessage, TServerMessage>,
    IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
    where TClient : ClientBase<TClient>
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
}