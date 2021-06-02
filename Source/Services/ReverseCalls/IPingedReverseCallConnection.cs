// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Threading;
using Google.Protobuf;
using Grpc.Core;

namespace Dolittle.Runtime.Services.ReverseCalls
{
    /// <summary>
    /// Defines a pinged reverse call connection that wraps a gRPC duplex streaming method call that is monitored and kept alive using ping messages.
    /// </summary>
    /// <typeparam name="TClientMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Client to the Runtime.</typeparam>
    /// <typeparam name="TServerMessage">Type of the <see cref="IMessage">messages</see> that is sent from the Runtime to the Client.</typeparam>
    public interface IPingedReverseCallConnection<TClientMessage, TServerMessage> : IDisposable
        where TClientMessage : IMessage, new()
        where TServerMessage : IMessage, new()
    {
        /// <summary>
        /// Gets the reader for the stream of messages from the Client to the Runtime.
        /// </summary>
        /// <remarks>
        /// The received pong messages from the Client will be filtered out of this stream.
        /// </remarks>
        IAsyncStreamReader<TClientMessage> RuntimeStream { get; }

        /// <summary>
        /// Gets the writer for the stream of messages from the Runtime to the Client.
        /// </summary>
        /// <remarks>
        /// The connection will send ping messages to the Client in between the messages written to this stream.
        /// </remarks>
        IAsyncStreamWriter<TServerMessage> ClientStream { get; }

        /// <summary>
        /// Gets the cancellation token that will be cancelled when the connection is no longer alive.
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}
