// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Embeddings.Store.MongoDB.Definition
{
    /// <summary>
    /// Defines a system that can convert to an from persisted and runtime representation of a embedding definition.
    /// </summary>
    public interface IConvertEmbeddingDefinition
    {
        /// <summary>
        /// Converts a runtime representation of an embedding definition to a the persisted mongodb representation.
        /// </summary>
        /// <param name="definition">The runtime representation of the embedding definition.</param>
        /// <returns>The persisted mongodb representation of the embedding definition.</returns>
        EmbeddingDefinition ToStored(Store.Definition.EmbeddingDefinition definition);

        /// <summary>
        /// Converts a persisted mongodb representation of an embedding definition to the runtime representation.
        /// </summary>
        /// <param name="definition">The persisted mongodb representation of the embedding definition.</param>
        /// <returns>The runtime representation of the embedding definition.</returns>
        Store.Definition.EmbeddingDefinition ToRuntime(EmbeddingDefinition definition);
    }
}
