// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Defines a system that can fetch embedding keys from the embedding store.
    /// </summary>
    public interface IFetchEmbeddingKeys
    {
        /// <summary>
        /// Try to get all keys for an embedding.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="IEnumerable{T}"/> of <see cref="ProjectionKey" />.</returns>
        Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, CancellationToken cancellationToken);

        /// <summary>
        /// Try to get all keys for an embedding.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="includeRemoved">Whether to get removed embedding too.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/>.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="IEnumerable{T}"/> of <see cref="ProjectionKey" />.</returns>
        Task<Try<IEnumerable<ProjectionKey>>> TryGetKeys(EmbeddingId embedding, bool includeRemoved, CancellationToken cancellationToken);
    }
}
