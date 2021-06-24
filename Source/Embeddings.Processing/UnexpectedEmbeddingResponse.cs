// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when an unexpected embedding response is received.
    /// </summary>
    public class UnexpectedEmbeddingResponse : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnexpectedEmbeddingResponse"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        /// <param name="responseCase">The <see cref="EmbeddingResponse.ResponseOneofCase"/> </param>
        public UnexpectedEmbeddingResponse(EmbeddingId embedding, EmbeddingResponse.ResponseOneofCase responseCase)
            : base($"Embedding {embedding.Value} returned an unexpected response case {responseCase}")
        {
        }
    }
}