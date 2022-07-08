// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store;

namespace Dolittle.Runtime.Embeddings.Processing;

/// <summary>
/// Exception that gets thrown when embedding defintion persisting failed.
/// </summary>
public class FailedPersistingEmbeddingDefinition : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FailedPersistingEmbeddingDefinition"/> class.
    /// </summary>
    /// <param name="embedding">The <see cref="EmbeddingId"/> </param>
    public FailedPersistingEmbeddingDefinition(EmbeddingId embedding)
        : base($"A failure occurred while persisting definition for embedding {embedding.Value}")
    {
    }
}