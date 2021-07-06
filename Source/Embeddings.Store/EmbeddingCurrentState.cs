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
    public record EmbeddingCurrentState(AggregateRootVersion Version, EmbeddingCurrentStateType Type, ProjectionState State, ProjectionKey Key)
    {
        /// <summary>
        /// Implicitly converts a <see cref="EmbeddingCurrentState" /> to <see cref="ProjectionCurrentState" />.
        /// </summary>
        /// <param name="current">The <see cref="EmbeddingCurrentState" />.</param>
        public static implicit operator ProjectionCurrentState(EmbeddingCurrentState current) => new(ProjectionTypeFromEmbeddingType(current.Type), current.State, current.Key);

        static ProjectionCurrentStateType ProjectionTypeFromEmbeddingType(EmbeddingCurrentStateType type)
            => type switch
            {
                EmbeddingCurrentStateType.CreatedFromInitialState => ProjectionCurrentStateType.CreatedFromInitialState,
                EmbeddingCurrentStateType.Persisted => ProjectionCurrentStateType.Persisted,
                EmbeddingCurrentStateType.Deleted => ProjectionCurrentStateType.CreatedFromInitialState,
                _ => throw new UnknownEmbeddingCurrentStateType(type)
            };
    };
}
