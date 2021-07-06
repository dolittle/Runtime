// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when a failure occurs in the head while handling a compare embedding call.
    /// </summary>
    public class EmbeddingRemoteCompareCallFailed : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmbeddingRemoteCompareCallFailed"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        public EmbeddingRemoteCompareCallFailed(EmbeddingId embedding, string reason)
            : base($"A failure occurred during the remote Compare call for embedding {embedding.Value}. {reason}")
        {
        }
    }
}