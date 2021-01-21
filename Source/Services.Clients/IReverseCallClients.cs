// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Services.Contracts;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that knows about <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.
    /// </summary>
    public interface IReverseCallClients
    {
        /// <summary>
        /// Gets a <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" /> that can mange the reverse calls.
        /// </summary>
        /// <param name="establishConnection">The <see cref="Func{TResult}" /> callback for connecting the client and establishing the <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}" />.</param>
        /// <param name="setConnectArguments">A delegate to set the <typeparamref name="TConnectArguments" /> on a <typeparamref name="TClientMessage" />.</param>
        /// <param name="getConnectResponse">A delegate to get the <typeparamref name="TConnectResponse" /> from a <typeparamref name="TServerMessage" />.</param>
        /// <param name="getMessageRequest">A delegate to get the <typeparamref name="TRequest" /> from the <typeparamref name="TServerMessage" />.</param>
        /// <param name="setMessageResponse">A delegate to set the <typeparamref name="TResponse" /> on a <typeparamref name="TClientMessage" />.</param>
        /// <param name="setArgumentsContext">A delegate to set the <see cref="ReverseCallArgumentsContext" /> on a <typeparamref name="TConnectArguments" />.</param>
        /// <param name="getRequestContext">A delegate to get the <see cref="ReverseCallRequestContext" /> from the <typeparamref name="TRequest" />.</param>
        /// <param name="setResponseContext">A delegate to set the <see cref="ReverseCallResponseContext" /> on a <typeparamref name="TResponse" />.</param>
        /// <param name="getPing">A delegate to get the <see cref="Ping" /> from the <typeparamref name="TServerMessage" />.</param>
        /// <param name="setPong">A delegate to set the <see cref="Ping" /> on a <typeparamref name="TClientMessage" />.</param>
        /// <param name="pingInterval">A <see cref="TimeSpan" /> for the interval between pings from the server.</param>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
        /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
        /// <returns>The <see cref="IReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</returns>
        IReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            Func<AsyncDuplexStreamingCall<TClientMessage, TServerMessage>> establishConnection,
            Action<TClientMessage, TConnectArguments> setConnectArguments,
            Func<TServerMessage, TConnectResponse> getConnectResponse,
            Func<TServerMessage, TRequest> getMessageRequest,
            Action<TClientMessage, TResponse> setMessageResponse,
            Action<TConnectArguments, ReverseCallArgumentsContext> setArgumentsContext,
            Func<TRequest, ReverseCallRequestContext> getRequestContext,
            Action<TResponse, ReverseCallResponseContext> setResponseContext,
            Func<TServerMessage, Ping> getPing,
            Action<TClientMessage, Pong> setPong,
            TimeSpan pingInterval = default)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}
