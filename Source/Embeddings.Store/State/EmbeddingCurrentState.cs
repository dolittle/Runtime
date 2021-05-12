// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store;

namespace Dolittle.Runtime.Embeddings.Store.State
{
    /// <summary>
    /// The current embedding state.
    /// </summary>
    /// <param name="Type">The type of the state.</param>
    /// <param name="State">The state.</param>
    /// <param name="Key">The key of the embedding.</param>
    public record EmbeddingCurrentState(EmbeddingCurrentStateType Type, EmbeddingState State, ProjectionKey Key);
}
