// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Dolittle.Runtime.Rudimentary;
using System.Threading;

namespace Dolittle.Runtime.Services;

public interface IInitiateReverseCallServices
{
    /// <summary>
    /// Initiates the connection.
    /// </summary>
    /// <param name="runtimeStream">The client to runtime stream.</param>
    /// <param name="clientStream">The runtime to client stream.</param>
    /// <param name="context">The server call context.</param>
    /// <param name="protocol">The protocol for converting reverse call messages.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
    /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
    /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
    /// <typeparam name="TRequest">Type of the requests sent from the server to the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TResponse">Type of the responses received from the client using <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/>.</typeparam>
    /// <typeparam name="TRuntimeConnectArguments">Type of the runtime representation of the connect arguments received from the client.</typeparam>
    /// <returns>The dispatcher and the runtime representation of the connect arguments.</returns>
    Task<Try<(IReverseCallDispatcher<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> dispatcher, TRuntimeConnectArguments arguments)>> Connect<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse, TRuntimeConnectArguments>(
        IAsyncStreamReader<TClientMessage> runtimeStream,
        IServerStreamWriter<TServerMessage> clientStream,
        ServerCallContext context,
        IReverseCallServiceProtocol<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse, TRuntimeConnectArguments> protocol,
        CancellationToken cancellationToken)
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
        where TConnectArguments : class
        where TConnectResponse : class
        where TRequest : class
        where TResponse : class
        where TRuntimeConnectArguments : class;
}