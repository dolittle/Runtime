// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store.State
{
    /// <summary>
    /// Defines the repository for the embeddings <see cref="ProjectionState" />.
    /// </summary>
    public interface IEmbeddingStates
    {
        /// <summary>
        /// Try to get a specific <see cref="EmbeddingState" />.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="key">The embedding key.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="EmbeddingState" />.</returns>
        Task<Try<EmbeddingState>> TryGet(EmbeddingId embedding, ProjectionKey key, CancellationToken token);

        /// <summary>
        /// Try to get all <see cref="EmbeddingState" />.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="EmbeddingState" />.</returns>
        Task<Try<IEnumerable<(EmbeddingState State, ProjectionKey Key)>>> TryGetAll(EmbeddingId embedding, CancellationToken token);

        /// <summary>
        /// Try to replace a specific <see cref="EmbeddingState" />.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="key">The embedding key.</param>
        /// <param name="state">The new embedding state.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{bool}"/> indicating whether the state was successfully replaced.</returns>
        Task<Try<bool>> TryReplace(EmbeddingId embedding, ProjectionKey key, EmbeddingState state, CancellationToken token);

        /// <summary>
        /// Try to mark a specific <see cref="EmbeddingState" /> as removed.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="key">The embedding key.</param>
        /// <param name="version">The <see cref="AggregateRootVersion"/>.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the state was successfully removed.</returns>
        Task<Try<bool>> TryMarkAsRemove(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, CancellationToken token);

        /// <summary>
        /// Try to drop the whole <see cref="EmbeddingState" /> collection.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns value indicating whether the embedding collection was successfully dropped.</returns>
        Task<Try> TryDrop(EmbeddingId embedding, CancellationToken token);
    }
}
