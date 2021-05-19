// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs in the head while handling a delete embedding call.
    /// </summary>
    public class EmbeddingRemoteDeleteCallFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingRemoteDeleteCallFailed"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        public EmbeddingRemoteDeleteCallFailed(EmbeddingId embedding, string reason)
            : base($"A failure occurred during the remote Delete call for embedding {embedding.Value}. {reason}")
        {
        }
    }
}