// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Store.State
{
    /// <summary>
    /// Represents the state of an embedding tied to an aggregate root version.
    /// </summary>
    /// <param name="State">The state.</param>
    /// <param name="Version">The aggregate root version the state was calculated from.</param>
    public record EmbeddingState(ProjectionState State, AggregateRootVersion Version, bool IsRemoved);
}
