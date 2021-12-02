
// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when a loop is detected in an embedding.
/// </summary>
public class EmbeddingLoopDetected : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmbeddingLoopDetected"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
    public EmbeddingLoopDetected(EmbeddingId embedding)
        : base($"Embedding loop was detected for embedding {embedding.Value}")
    {
    }
}