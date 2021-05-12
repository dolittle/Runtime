// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    public class FailedToGetEmbeddingState : Exception
    {
        public FailedToGetEmbeddingState(EmbeddingId embedding, ProjectionKey key)
            : base($"Failed to get embedding's state: Id: {embedding} Key: {key}")
        {
        }
    }
}
