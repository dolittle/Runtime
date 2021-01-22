// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a client for reverse calls coming from server to client.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public interface IReverseCallClient<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Gets the connect response.
        /// </summary>
        TConnectResponse ConnectResponse { get; }

        /// <summary>
        /// Connect to server.
        /// </summary>
        /// <param name="connectArguments">The connection arguments.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns whether a connection response was received.</returns>
        Task<bool> Connect(TConnectArguments connectArguments, CancellationToken token);

        /// <summary>
        /// Handle a call.
        /// </summary>
        /// <param name="callback">The <see cref="Func{T1, T2, TReturn}">callback</see> for requests coming from server.</param>
        /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task Handle(Func<TRequest, CancellationToken, Task<TResponse>> callback, CancellationToken token);
    }
}