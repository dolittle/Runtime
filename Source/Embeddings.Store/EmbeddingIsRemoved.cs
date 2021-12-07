// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when you try to get an <see cref="EmbeddingState" /> that is removed.
/// </summary>
public class EmbeddingIsRemoved : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingIsRemoved" /> class.
    /// </summary>
    /// <param name="embedding">The embedding id.</param>
    /// <param name="key">The key to the embedding.</param>
    /// <param name="version">The <see cref="AggregateRootVersion"/> of the removed embedding.</param>
    public EmbeddingIsRemoved(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
        : base($"Can't get embedding {embedding.Value} with key {key.Value}, as it's already removed. Aggregate root version of the removed embedding is {version.Value}")
    {
    }
}