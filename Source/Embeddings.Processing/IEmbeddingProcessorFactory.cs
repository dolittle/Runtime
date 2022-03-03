// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Execution;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Defines a factory that can create <see cref="IEmbeddingProcessor" />.
/// </summary>
public interface IEmbeddingProcessorFactory
{
    /// <summary>
    /// Creates an <see cref="IEmbeddingProcessor" />.
    /// </summary>
    /// <param name="embeddingId">The embedding identifier.</param>
    /// <param name="embedding">The embedding.</param>
    /// <param name="initialState">The initial state of the embedding.</param>
    /// <param name="executionContext">The execution context to run the processor in.</param>
    /// <returns>The created <see cref="IEmbeddingProcessor" />.</returns>
    IEmbeddingProcessor Create(EmbeddingId embeddingId, IEmbedding embedding, ProjectionState initialState, ExecutionContext executionContext);
}
