// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using MongoDB.Driver;

namespace Dolittle.Runtime.Embeddings.Store.MongoDB
{
    /// <summary>
    /// Defines a system that knows projections.
    /// </summary>
    public interface IEmbeddings : IEmbeddingsConnection
    {
        /// <summary>
        /// Gets the projection states collection for an embedding.
        /// </summary>
        /// <param name="projectionId">The <see cref="ProjectionId" />.</param>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="State.Projection" />.</returns>
        Task<IMongoCollection<State.Embedding>> GetStates(EmbeddingId embeddingId, CancellationToken token);

        /// <summary>
        /// Gets the embedding definitions collection for an embedding.
        /// </summary>
        /// <param name="token">The <see cref="CancellationToken" />.</param>
        /// <returns>A <see cref="Task" /> that, when resolved, returns a <see cref="IMongoCollection{TDocument}" /> with <see cref="Definition.EmbeddingDefinition" />.</returns>
        Task<IMongoCollection<Definition.EmbeddingDefinition>> GetDefinitions(CancellationToken token);
    }
}
