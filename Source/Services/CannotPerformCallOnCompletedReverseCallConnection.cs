// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services
{
    /// <summary>
    /// Exception that gets thrown when attempting to perfom a <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.Call"/> after the client has closed the connection.
    /// </summary>
    public class CannotPerformCallOnCompletedReverseCallConnection : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CannotPerformCallOnCompletedReverseCallConnection"/> class.
        /// </summary>
        public CannotPerformCallOnCompletedReverseCallConnection()
            : base("Cannot perform calls using a ReverseCallDispatcher after the client has closed the connection.")
        {
        }
    }
}