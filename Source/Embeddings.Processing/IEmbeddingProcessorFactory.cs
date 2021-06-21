// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.ApplicationModel;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a factory that can create <see cref="IEmbeddingProcessor" />.
    /// </summary>
    public interface IEmbeddingProcessorFactory
    {
        /// <summary>
        /// Creates an <see cref="IEmbeddingProcessor" />.
        /// </summary>
        /// <param name="tenant">The tenant identifier.</param>
        /// <param name="embeddingId">The embedding identifier.</param>
        /// <param name="embedding">The embedding.</param>
        /// <returns>The created <see cref="IEmbeddingProcessor" />.</returns>
        IEmbeddingProcessor Create(TenantId tenant, EmbeddingId embeddingId, IEmbedding embedding, ProjectionState initialState);
    }
}
