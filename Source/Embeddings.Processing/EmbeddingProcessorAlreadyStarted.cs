// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when starting an <see cref="EmbeddingProcessor"/> more than once.
    /// </summary>
    public class EmbeddingProcessorAlreadyStarted : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingProcessorAlreadyStarted"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        public EmbeddingProcessorAlreadyStarted(EmbeddingId embedding)
            : base($"EmbeddingProcessor for embedding {embedding.Value} is already started")
        {
        }
    }
}