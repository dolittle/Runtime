// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Defines a system that can fetch embedding states from the embedding store.
    /// </summary>
    public interface IFetchEmbeddingsStates
    {
        /// <summary>
        /// Try to get the state of an embedding by id and key.
        /// </summary>
        /// <param name="embedding">The embedding id.</param>
        /// <param name="key">The projection key.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns <see cref="Try{TResult}" /> of <see cref="EmbeddingCurrentState" />.</returns>
        Task<Try<EmbeddingCurrentState>> TryGet(EmbeddingId embedding, ProjectionKey key, CancellationToken cancellationToken);
    }
}
