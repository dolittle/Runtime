// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when the embedding store failed to replace an embedding.
/// </summary>
public class FailedToReplaceEmbedding : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="FailedToReplaceEmbedding" /> class.
    /// </summary>
    /// <param name="embedding">The embedding identifier.</param>
    /// <param name="key">The projection key</param>
    /// <param name="version">The aggregate root version.</param>
    /// <param name="state">The new projection state.</param>
    public FailedToReplaceEmbedding(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state)
        : base($"Failed to replace embedding with id {embedding.Value}, key {key.Value} and aggregate root version {version.Value} with state {state.Value}")
    {
    }
}