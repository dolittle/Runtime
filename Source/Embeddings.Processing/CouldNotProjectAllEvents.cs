
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Exception that gets thrown when we could not project all events. 
    /// </summary>
    public class CouldNotProjectAllEvents : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotProjectAllEvents"/> class.
        /// </summary>
        /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
        /// <param name="inner">The inner<see cref="Exception"/> </param>
        public CouldNotProjectAllEvents(EmbeddingId embedding, Exception inner)
            : base($"Not all events could be projected for embedding {embedding.Value}", inner)
        {
        }
    }
}