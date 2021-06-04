// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;

namespace Dolittle.Runtime.Services.Clients
{
    /// <summary>
    /// Defines the signature of a reverse call request handler.
    /// </summary>
    /// <param name="request">The request to be handled.</param>
    /// <param name="cancellationToken">A cancellation token indicating if the request handling should be cancelled.</param>
    /// <typeparam name="TRequest">The type of the requests.</typeparam>
    /// <typeparam name="TResponse">The expected type of the responses.</typeparam>
    /// <returns>A task that, when resolved returns the result of the request.</returns>
    public delegate Task<TResponse> ReverseCallHandler<TRequest,TResponse>(TRequest request, CancellationToken cancellationToken);
}