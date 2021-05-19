// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Defines a system that can convert to an from persisted and runtime representation of a embedding definition.
    /// </summary>
    public interface IConvertEmbeddingDefinition
    {
        EmbeddingDefinition ToStored(Store.Definition.EmbeddingDefinition definition);
        Store.Definition.EmbeddingDefinition ToRuntime(EmbeddingDefinition definition);
    }
}
