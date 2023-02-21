// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using ExecutionContext = Dolittle.Runtime.Execution.ExecutionContext;

namespace Dolittle.Runtime.Services.Clients;

/// <summary>
/// Defines a client for reverse calls coming from server to client.
/// </summary>
/// <typeparam name="TConnectArguments">Type of the arguments that are sent along with the initial Connect call.</typeparam>
/// <typeparam name="TConnectResponse">Type of the response that is received after the initial Connect call.</typeparam>
/// <typeparam name="TRequest">Type of the requests sent from the server to the client.</typeparam>
/// <typeparam name="TResponse">Type of the responses received from the client.</typeparam>
public interface IReverseCallClient<TConnectArguments, TConnectResponse, TRequest, TResponse> : IDisposable
    where TConnectArguments : class
    where TConnectResponse : class
    where TRequest : class
    where TResponse : class
{
    /// <summary>
    /// Gets the connect response.
    /// </summary>
    TConnectResponse ConnectResponse { get; }

    /// <summary>
    /// Connect to server.
    /// </summary>
    /// <param name="connectArguments">The connection arguments.</param>
    /// <param name="executionContext">The base execution context of this reverse call.</param>
    /// <param name="token">The <see cref="CancellationToken" />.</param>
    /// <returns>A <see cref="Task" /> that, when resolved, returns whether a connection response was received.</returns>
    Task<bool> Connect(TConnectArguments connectArguments, ExecutionContext executionContext, CancellationToken token);

    /// <summary>
    /// Handle a call.
    /// </summary>
    /// <param name="callback">The <see cref="ReverseCallHandler{TRequest, TResponse}"/> for requests coming from server.</param>
    /// <param name="token">Optional. A <see cref="CancellationToken" /> to cancel the operation.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task Handle(ReverseCallHandler<TRequest, TResponse> callback, CancellationToken token);
}
