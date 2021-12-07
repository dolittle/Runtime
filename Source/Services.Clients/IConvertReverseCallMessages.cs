// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Protobuf.Contracts;
using Dolittle.Services.Contracts;
using Google.Protobuf;

namespace Dolittle.Runtime.Services.Clients;

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
    /// Sets the <see cref="ReverseCallArgumentsContext"/> in a <typeparamref name="TConnectArguments"/>.
    /// </summary>
    /// <param name="context">The <see cref="ReverseCallArgumentsContext"/> to set.</param>
    /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to set the context in.</param>
    void SetConnectArgumentsContext(ReverseCallArgumentsContext context, TConnectArguments arguments);

    /// <summary>
    /// Sets the <typeparamref name="TConnectArguments"/> in a <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="arguments">The <typeparamref name="TConnectArguments"/> to set.</param>
    /// <param name="message">The <typeparamref name="TClientMessage"/> to set the arguments in.</param>
    void SetConnectArguments(TConnectArguments arguments, TClientMessage message);

    /// <summary>
    /// Gets the <typeparamref name="TConnectResponse"/> from a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TServerMessage"/> to get the connect response from.</param>
    /// <returns>The <typeparamref name="TConnectResponse"/> in the message.</returns>
    TConnectResponse GetConnectResponse(TServerMessage message);

    /// <summary>
    /// Gets the optional <see cref="Failure" /> from a <typeparamref name="TConnectResponse"/>.
    /// </summary>
    /// <param name="response">The <typeparamref name="TConnectResponse"/> to get the failure from.</param>
    /// <returns>The <see cref="Failure" /> from the <typeparamref name="TConnectResponse"/> or null if there was no failure.</returns>
    Failure GetFailureFromConnectResponse(TConnectResponse response);

    /// <summary>
    /// Gets the <see cref="Ping"/> from a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TServerMessage"/> to get the ping from.</param>
    /// <returns>The <see cref="Ping"/> in the message.</returns>
    Ping GetPing(TServerMessage message);

    /// <summary>
    /// Sets the <see cref="Pong"/> in a <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="arguments">The <see cref="Pong"/> to set.</param>
    /// <param name="message">The <typeparamref name="TClientMessage"/> to set the pong in.</param>
    void SetPong(Pong pong, TClientMessage message);

    /// <summary>
    /// Gets the <typeparamref name="TRequest"/> from a <typeparamref name="TServerMessage"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TServerMessage"/> to get the request from.</param>
    /// <returns>The <typeparamref name="TRequest"/> in the message.</returns>
    TRequest GetRequest(TServerMessage message);

    /// <summary>
    /// Gets the <see cref="ReverseCallRequestContext"/> from a <typeparamref name="TRequest"/>.
    /// </summary>
    /// <param name="message">The <typeparamref name="TRequest"/> to get the context from.</param>
    /// <returns>The <see cref="ReverseCallRequestContext"/> in the request.</returns>
    ReverseCallRequestContext GetRequestContext(TRequest message);

    /// <summary>
    /// Sets the <see cref="ReverseCallResponseContext"/> in a <typeparamref name="TResponse"/>.
    /// </summary>
    /// <param name="context">The <see cref="ReverseCallResponseContext"/> to set.</param>
    /// <param name="response">Tge <typeparamref name="TResponse"/> to set the context in.</param>
    void SetResponseContext(ReverseCallResponseContext context, TResponse response);

    /// <summary>
    /// Sets the <typeparamref name="TResponse"/> in a <typeparamref name="TClientMessage"/>.
    /// </summary>
    /// <param name="arguments">The <typeparamref name="TResponse"/> to set.</param>
    /// <param name="message">The <typeparamref name="TClientMessage"/> to set the response in.</param>
    void SetResponse(TResponse response, TClientMessage message);
}