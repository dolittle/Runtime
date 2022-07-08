// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Store.Definition;

namespace Dolittle.Runtime.Embeddings.Store;

/// <summary>
/// Exception that gets thrown when a <see cref="EmbeddingDefinition" /> does not exist.
/// </summary>
public class EmbeddingDefinitionDoesNotExist : Exception
{
    /// <summary>
    /// Initializes an instance of the <see cref="EmbeddingDefinitionDoesNotExist" /> class.
    /// </summary>
    /// <param name="embedding">The embedding id.</param>
    public EmbeddingDefinitionDoesNotExist(EmbeddingId embedding)
        : base($"Embedding {embedding.Value} doesn't have a definition")
    {
    }
}