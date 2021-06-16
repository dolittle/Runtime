// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines a system that can create instances of <see cref="IReverseCallClient{TConnectArguments, TConnectResponse, TRequest, TResponse}"/>.
    /// </summary>
    public interface IReverseCallClients
    {
        /// <summary>
        /// Create a reverse call client that connects to the provided host:port with the provided protocol.
        /// </summary>
        /// <param name="protocol">The protocol for this reverse call.</param>
        /// <param name="host">The host to connect to.</param>
        /// <param name="port">The port to connect to.</param>
        /// <param name="pingInterval">A <see cref="TimeSpan" /> for the interval between pings from the server.</param>
        /// <typeparam name="TClient">Type of the client to use for calls to the server.</typeparam>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
        /// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
        /// <returns>A new reverse call client.</returns>
        IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> GetFor<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            IReverseCallClientProtocol<TClient, TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> protocol,
            string host,
            int port,
            TimeSpan pingInterval = default)
            where TClient : ClientBase<TClient>
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}
