// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Exception that gets thrown when <see cref="ReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Reject(TConnectResponse, System.Threading.CancellationToken)" />
    /// is called more than once.
    /// </summary>
    public class ReverseCallDispatcherAlreadyRejected : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReverseCallDispatcherAlreadyRejected"/> class.
        /// </summary>
        public ReverseCallDispatcherAlreadyRejected()
            : base("Reverse Call Dispatcher can only reject once")
        {
        }
    }
}