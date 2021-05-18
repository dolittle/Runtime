// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Store
{
    public class FailedToReplaceEmbedding : Exception
    {
        public FailedToReplaceEmbedding(EmbeddingId embedding, ProjectionKey key, AggregateRootVersion version, ProjectionState state)
            : base($"Failed to replace embedding with id {embedding}, key {key} and aggregate root version {version} with state {state}")
        {
        }
    }
}