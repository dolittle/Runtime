// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    public class FailedToRemoveEmbedding : Exception
    {
        public FailedToRemoveEmbedding(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version)
            : base($"Failed to remove embedding with id {embedding.Value}, key {key.Value} and aggregate root version {version.Value}")
        {
        }
    }
}
