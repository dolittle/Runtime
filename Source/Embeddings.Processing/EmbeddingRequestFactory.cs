// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;

namespace Dolittle.Runtime.Embeddings.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IEmbeddingRequestFactory" />.
    /// </summary>
    public class EmbeddingRequestFactory : IEmbeddingRequestFactory
    {
        /// <inheritdoc/>
        public EmbeddingRequest Create(ProjectionCurrentState current, UncommittedEvent @event)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public EmbeddingRequest Create(EmbeddingCurrentState current, ProjectionState desiredState)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public EmbeddingRequest Create(EmbeddingCurrentState current)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(ProjectionCurrentState current, UncommittedEvent @event)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current, ProjectionState desiredState)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current)
        {
            throw new NotImplementedException();
        }
    }
}
