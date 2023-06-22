// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Protobuf;
using Dolittle.Services.Contracts;
using Google.Protobuf;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Defines a converter that reads and writes parts of duplex streaming messages using the reverse call protocol.
/// </summary>
/// <typeparam name="TClientMessage">Type of the messages that is sent from the client to the server.</typeparam>
/// <typeparam name="TServerMessage">Type of the messages that is sent from the server to the client.</typeparam>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
public interface IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
    where TClientMessage : IMessage, new()
    where TServerMessage : IMessage, new()
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Gets the <typeparamref name="TConnectResponse"/> from a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TClientMessage"/> to get the connect arguments from.</param>
    /// <returns>The <typeparamref name="TConnectResponse"/> in the message.</returns>
    TConnectArguments? GetConnectArguments(TClientMessage message);

    /// <summary>
    /// Sets the <typeparamref name="TConnectResponse"/> in a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="arguments">The <typeparamref name="TConnectResponse"/> to set.</param>
    /// <param name="message">The <typeparamref name="TServerMessage"/> to set the response in.</param>
    void SetConnectResponse(TConnectResponse arguments, TServerMessage message);

    /// <summary>
    /// Sets the <typeparamref name="TRequest"/> in a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="request">The <typeparamref name="TRequest"/> to set.</param>
    /// <param name="message">The <typeparamref name="TServerMessage"/> to set the request in.</param>
    void SetRequest(TRequest request, TServerMessage message);

    /// <summary>
    /// Gets the <typeparamref name="TResponse"/> from the <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TClientMessage"/>.</param>
    /// <returns>The <typeparamref name="TResponse"/>.</returns>
    TResponse? GetResponse(TClientMessage message);

    /// <summary>
    /// Gets the <see cref="ReverseCallArgumentsContext" /> from the <typeparamref name="TConnectArguments"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TConnectArguments"/>.</param>
    /// <returns>The <see cref="ReverseCallArgumentsContext" />.</returns>
    ReverseCallArgumentsContext? GetArgumentsContext(TConnectArguments message);

    /// <summary>
    /// Sets the <see cref="ReverseCallRequestContext"/> in a <typeparamref name="TRequest"/>.
    /// </summary>
    /// <param name="context">The <see cref="ReverseCallRequestContext"/> to set.</param>
    /// <param name="request">Tge <typeparamref name="TRequest"/> to set the context in.</param>
    void SetRequestContext(ReverseCallRequestContext context, TRequest request);

    /// <summary>
    /// Gets the <see cref="ReverseCallResponseContext"/> from a <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TResponse"/> to get the context from.</param>
    /// <returns>The <see cref="ReverseCallResponseContext"/> in the request.</returns>
    ReverseCallResponseContext? GetResponseContext(TResponse message);

    /// <summary>
    /// Sets the <see cref="Ping" /> in the <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TServerMessage"/>.</param>
    /// <param name="ping">The <see cref="Ping" />.</param>
    void SetPing(TServerMessage message, Ping ping);

    /// <summary>
    /// Gets the <see cref="Pong"/> from a <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TClientMessage"/> to get the pong from.</param>
    /// <returns>The <see cref="Pong"/> in the message.</returns>
    Pong? GetPong(TClientMessage message);

    bool SupportsGracefulShutdown => false;
    
    /// <summary>
    /// If the protocol supports it, and it is of the correct type, get <see cref="InitiateDisconnect" /> from the <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    InitiateDisconnect? GetInitiateDisconnect(TClientMessage message) => null;
    
    /// <summary>
    /// Creates a <typeparamref name="TConnectResponse"/> signifying a failed connection.
    /// </summary>
    /// <param name="failureMessage">The reason for failure.</param>
    /// <returns>The failed <typeparamref name="TConnectResponse"/>.</returns>    
    TConnectResponse CreateFailedConnectResponse(FailureReason failureMessage);
}
