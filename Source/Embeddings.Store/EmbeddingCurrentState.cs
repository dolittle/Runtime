// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;

namespace Dolittle.Runtime.Embeddings.Store
{
    /// <summary>
    /// Represents the current state of an embedding tied to an aggregate root version.
    /// </summary>
    /// <param name="Version">The aggregate root version the state was calculated from.</param>
    /// <param name="Type">The type of the state.</param>
    /// <param name="State">The state.</param>
    /// <param name="Key">The key of the projection.</param>
    public record EmbeddingCurrentState(AggregateRootVersion Version, ProjectionCurrentStateType Type, ProjectionState State, ProjectionKey Key) : ProjectionCurrentState(Type, State, Key);
}
