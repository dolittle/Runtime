// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Defines a system that can create pinged reverse call connections from gRPC duplex streaming method calls. 
    /// </summary>
    public interface IKeepReverseCallConnectionsAlive
    {
        /// <summary>
        /// Creates a pinged reverse call connection from a gRPC duplex streaming method call.
        /// </summary>
        /// <param name="requestId">The request id for the gRPC method call.</param>
        /// <param name="runtimeStream">The <see cref="IAsyncStreamReader{TClientMessage}"/> to read messages to the Runtime.</param>
        /// <param name="clientStream">The <see cref="IServerStreamWriter{TServerMessage}"/> to write messages to the Client.</param>
        /// <param name="context">The <see cref="ServerCallContext"/> of the method call.</param>
        /// <param name="messageConverter">The <see cref="MethodConverter"/> to use for decoding the connect arguments and reading the desired ping interval from.</param>
        /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
        /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
        /// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
        /// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
        /// <typeparam name="TRequest">Type of the requests sent from the Runtime to the Client.</typeparam>
        /// <typeparam name="TResponse">Type of the responses sent from the Client to the Runtime.</typeparam>
        /// <returns>A <see cref="IPingedReverseCallConnection{TClientMessage, TServerMessage}"/> that will be kept alive using ping messages.</returns>
        IPingedReverseCallConnection<TClientMessage, TServerMessage> CreatePingedReverseCallConnection<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse>(
            RequestId requestId,
            IAsyncStreamReader<TClientMessage> runtimeStream,
            IAsyncStreamWriter<TServerMessage> clientStream,
            ServerCallContext context,
            IConvertReverseCallMessages<TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse> messageConverter)
            where TClientMessage : IMessage, new()
            where TServerMessage : IMessage, new()
            where TConnectArguments : class
            where TConnectResponse : class
            where TRequest : class
            where TResponse : class;
    }
}
