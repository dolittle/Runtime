// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines a duplex streaming method call.
/// </summary>
/// <typeparam name="TClient">Type of the client to use for calls to the server.</typeparam>
/// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the client to the server.</typeparam>
/// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the server to the client.</typeparam>
public interface ICanCallADuplexStreamingMethod<in TClient, TClientMessage, TServerMessage>
    where TClient : ClientBase<TClient>
    where TClientMessage : IMessage
    where TServerMessage : IMessage
{
    /// <summary>
    /// Performs the duplex streaming method call on the server using the provided channel and call options.
    /// </summary>
    /// <param name="client">The <typeparamref name="TClient"/> to use for the call.</param>
    /// <param name="callOptions">The <see cref="CallOptions"/> to use for the call.</param>
    /// <returns>The <see cref="AsyncDuplexStreamingCall{TRequest, TResponse}"/> representing the method call.</returns>
    AsyncDuplexStreamingCall<TClientMessage, TServerMessage> Call(TClient client, CallOptions callOptions);
}
