// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB
{
    /// <summary>
    /// Defines a connection to the Embeddings.
    /// </summary>
    public interface IEmbeddingsConnection
    {
        /// <summary>
        /// Starts a client session.
        /// </summary>
        /// <param name="options">The <see cref="ClientSessionOptions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>The <see cref="IClientSessionHandle" />.</returns>
        IClientSessionHandle StartSession(ClientSessionOptions options = default, CancellationToken cancellationToken);

        /// <summary>
        /// Starts a client session.
        /// </summary>
        /// <param name="options">The <see cref="ClientSessionOptions" />.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns the <see cref="IClientSessionHandle" />.</returns>
        Task<IClientSessionHandle> StartSessionAsync(ClientSessionOptions options = default, CancellationToken cancellationToken);
    }
}
