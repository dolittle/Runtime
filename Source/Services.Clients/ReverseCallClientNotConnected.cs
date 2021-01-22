// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Exception that gets thrown when the <see cref="ReverseCallClient{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Handle(Func{TRequest, System.Threading.CancellationToken, System.Threading.Tasks.Task{TResponse}}, System.Threading.CancellationToken)" />
    /// is called before it has successfully established a connection.
    /// </summary>
    public class ReverseCallClientNotConnected : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallClientNotConnected"/> class.
        /// </summary>
        public ReverseCallClientNotConnected()
            : base($"Cannot handle Reverse Call Client before it has established a connection")
        {
        }
    }
}