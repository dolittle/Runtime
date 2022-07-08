// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when a <see cref="EmbeddingState" /> does not exist.
/// </summary>
public class EmbeddingStateDoesNotExist : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingStateDoesNotExist" /> class.
    /// </summary>
    /// <param name="embedding">The embedding id.</param>
    /// <param name="key">The key to the embedding.</param>
    public EmbeddingStateDoesNotExist(EmbeddingId embedding, ProjectionKey key)
        : base($"An embedding state for embedding {embedding.Value} with key {key.Value} does not exist")
    {
    }
}