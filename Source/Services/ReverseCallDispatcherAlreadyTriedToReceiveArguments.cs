// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Services;

/// <summary>
/// Exception that gets thrown when <see cref="ReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}.ReceiveArguments(System.Threading.CancellationToken)" />
/// is called more than once.
/// </summary>
public class ReverseCallDispatcherAlreadyTriedToReceiveArguments : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReverseCallDispatcherAlreadyTriedToReceiveArguments"/> class.
    /// </summary>
    public ReverseCallDispatcherAlreadyTriedToReceiveArguments()
        : base("Reverse Call Dispatcher can only receive arguments once")
    {
    }
}