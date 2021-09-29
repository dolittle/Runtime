// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when performing operations on an <see cref="EmbeddingProcessor"/> that is not started.
    /// </summary>
    public class EmbeddingProcessorNotStarted : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessorNotStarted"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        public EmbeddingProcessorNotStarted(EmbeddingId embedding)
            : base($"EmbeddingProcessor for embedding {embedding.Value} is not started")
        {
        }
    }
}