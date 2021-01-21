// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a system that provides trackers for reverse calls from server to client.
    /// </summary>
    public interface IReverseCallDispatchers
    {
        /// <summary>
        /// Get a <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/> for specific request and response streams.
        /// </summary>
        /// <param name="clientStream">The <see cref="IAsyncStreamReader{TClientMessage}"/> to read client messages from.</param>
        /// <param name="serverStream">The <see cref="IServerStreamWriter{TServerMessage}"/> to write server messages to.</param>
        /// <param name="context">The connection <see cref="ServerCallContext">context</see>.</param>
        /// <param name="getConnectArguments">A delegate to get the <typeparamref name="TConnectArguments"/> from a <typeparamref name="TClientMessage"/>.</param>
        /// <param name="setConnectResponse">A delegate to set the <typeparamref name="TConnectResponse"/> on a <typeparamref name="TServerMessage"/>.</param>
        /// <param name="setMessageRequest">A delegate to set the <typeparamref name="TRequest"/> on a <typeparamref name="TServerMessage"/>.</param>
        /// <param name="getMessageResponse">A delegate to get the <typeparamref name="TResponse"/> from a <typeparamref name="TClientMessage"/>.</param>
        /// <param name="getArgumentsContext">A delegate to get the <see cref="ReverseCallArgumentsContext"/> from a <typeparamref name="TConnectArguments"/>.</param>
        /// <param name="setRequestContext">A delegate to set the <see cref="ReverseCallRequestContext"/> on a <typeparamref name="TRequest"/>.</param>
        /// <param name="getResponseContex">A delegate to get the <see cref="ReverseCallResponseContext"/> from a <typeparamref name="TResponse"/>.</param>
        /// <param name="setPing">A delegate to set the <see cref="Ping" /> on a <typeparamref name="TServerMessage" />.</param>
        /// <param name="getPong">A delegate to get the <see cref="Pong" /> from a <typeparamref name="TClientMessage" />.</param>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
        /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
        /// <returns>A <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.</returns>
        IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
                IAsyncStreamReader<TClientMessage> clientStream,
                IServerStreamWriter<TServerMessage> serverStream,
                ServerCallContext context,
                Func<TClientMessage, TConnectArguments> getConnectArguments,
                Action<TServerMessage, TConnectResponse> setConnectResponse,
                Action<TServerMessage, TRequest> setMessageRequest,
                Func<TClientMessage, TResponse> getMessageResponse,
                Func<TConnectArguments, ReverseCallArgumentsContext> getArgumentsContext,
                Action<TRequest, ReverseCallRequestContext> setRequestContext,
                Func<TResponse, ReverseCallResponseContext> getResponseContex,
                Action<TServerMessage, Ping> setPing,
                Func<TClientMessage, Pong> getPong)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}