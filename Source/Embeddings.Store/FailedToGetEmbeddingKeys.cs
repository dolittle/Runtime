// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when the embedding store failed to get the persisted embedding keys for an embedding.
/// </summary>
public class FailedToGetEmbeddingKeys : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="FailedToGetEmbeddingKeys" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    public FailedToGetEmbeddingKeys(EmbeddingId embedding)
        : base($"Failed to get embedding's keys, EmbeddingId: {embedding.Value}")
    {
    }
}