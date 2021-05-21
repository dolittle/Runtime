// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store
{
    public class FailedToGetEmbeddingKeys : Exception
    {
        public FailedToGetEmbeddingKeys(EmbeddingId embedding)
            : base($"Failed to get embedding's keys, EmbeddingId: {embedding.Value}")
        {
        }
    }
}
