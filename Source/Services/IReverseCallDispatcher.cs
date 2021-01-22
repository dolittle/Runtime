// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Defines a dispatcher that is capable tracking calls from server to client.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    public interface IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
    {
        /// <summary>
        /// Gets the <typeparamref name="TConnectArguments"/> received from the initial Connect call from the client.
        /// </summary>
        TConnectArguments Arguments { get; }

        /// <summary>
        /// Waits for the initial Connect call arguments from the client.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns whether arguments was received from the client.</returns>
        Task<bool> ReceiveArguments(CancellationToken cancellationToken);

        /// <summary>
        /// Accepts the incoming connection from the client. This sends the <typeparamref name="TConnectResponse"/> back to the client, and starts handling of calls.
        /// </summary>
        /// <param name="response">The <typeparamref name="TConnectResponse"/> to send to the client.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of handling of calls.</returns>
        Task Accept(TConnectResponse response, CancellationToken cancellationToken);

        /// <summary>
        /// Rejects the incoming connection from the client. This sends the <typeparamref name="TConnectResponse"/> back to the client, and closes the connection.
        /// </summary>
        /// <param name="response">The <typeparamref name="TConnectResponse"/> to send to the client.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation of closing the connection.</returns>
        Task Reject(TConnectResponse response, CancellationToken cancellationToken);

        /// <summary>
        /// Dispatch a call to the client.
        /// </summary>
        /// <param name="request">The <typeparamref name="TRequest"/> to send to the client.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task"/> that, when resolved, returns the <typeparamref name="TResponse"/> from the client.</returns>
        Task<TResponse> Call(TRequest request, CancellationToken cancellationToken);
    }
}
