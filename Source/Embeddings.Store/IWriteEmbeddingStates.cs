// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Defines a system that can write embedding states to the embedding store.
    /// </summary>
    public interface IWriteEmbeddingStates
    {
        /// <summary>
        /// Try to replace a specific embedding state by key.
        /// </summary>
        /// <param name="embedding">The embeddding id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="state">The new projection state.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully replaced.</returns>
        Task<Try> TryReplace(EmbeddingId embedding, ProjectionKey key, ProjectionState state, CancellationToken cancellationToken);

        /// <summary>
        /// Try to remove a specific embedding state by key.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully removed.</returns>
        Task<Try> TryRemove(EmbeddingId embedding, ProjectionKey key, CancellationToken cancellationToken);
    }
}