// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs while deleting an embedding.
    /// </summary>
    public class FailedDeleteEmbedding : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedDeleteEmbedding"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        public FailedDeleteEmbedding(EmbeddingId embedding, string reason)
            : base($"Embedding {embedding.Value} failed while deleting. {reason}")
        {
        }
    }
}