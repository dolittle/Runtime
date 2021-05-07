// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Defines a factory that creates embedding requests. 
    /// </summary>
    public interface IEmbeddingRequestFactory
    {
        /// <summary>
        /// Creates an <see cref="EmbeddingRequest" /> for projections.
        /// </summary>
        /// <param name="current">The <see cref="ProjectionCurrentState" />.</param>
        /// <param name="event">The <see cref="UncommittedEvent" />.</param>
        /// <returns>A projection <see cref="Try{EmbeddingRequest}" />.</returns>
        Try<EmbeddingRequest> TryCreate(ProjectionCurrentState current, UncommittedEvent @event);

        /// <summary>
        /// Creates an <see cref="EmbeddingRequest" /> for comparisons.
        /// </summary>
        /// <param name="current">The <see cref="EmbeddingCurrentState" />.</param>
        /// <param name="desiredState">The desired <see cref="ProjectionState" />.</param>
        /// <returns>A compare <see cref="Try{EmbeddingRequest}" />.</returns>
        Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current, ProjectionState desiredState);

        /// <summary>
        /// Creates an <see cref="EmbeddingRequest" /> for deleting.
        /// </summary>
        /// <param name="current">The <see cref="EmbeddingCurrentState" />.</param>
        /// <returns>A delete <see cref="Try{EmbeddingRequest}" />.</returns>
        Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current);
    }
}
