// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when the embedding store failed to get the state of an embedding.
/// </summary>
public class FailedToGetEmbeddingState : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="FailedToGetEmbeddingState" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="key">The projection key.</param>
    public FailedToGetEmbeddingState(EmbeddingId embedding, ProjectionKey key)
        : base($"Failed to get embedding's state: Id: {embedding.Value} Key: {key.Value}")
    {
    }

    /// <summary>
    /// Initializes an instance of the <see cref="FailedToGetEmbeddingState" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    public FailedToGetEmbeddingState(EmbeddingId embedding)
        : base($"Failed to get embedding's state: Id: {embedding.Value}")
    {
    }
}