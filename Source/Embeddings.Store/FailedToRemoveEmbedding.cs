// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when the embedding store failed to remove an embedding.
/// </summary>
public class FailedToRemoveEmbedding : Exception
{

    /// <summary>
    /// Initializes an instance of the <see cref="FailedToRemoveEmbedding" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="key">The projection key</param>
    /// <param name="version">The aggregate root version.</param>
    public FailedToRemoveEmbedding(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
        : base($"Failed to remove embedding with id {embedding.Value}, key {key.Value} and aggregate root version {version.Value}")
    {
    }
}
