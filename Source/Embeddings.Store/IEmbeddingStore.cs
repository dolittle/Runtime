// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Defines a system that can knows about persisted embeddings in the embedding store.
    /// </summary>
    public interface IEmbeddingStore : IFetchEmbeddingsStates, IWriteEmbeddingStates
    {
    }
}