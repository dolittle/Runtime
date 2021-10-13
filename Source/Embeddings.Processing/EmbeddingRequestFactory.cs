// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Protobuf;
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
            => new()
            {
                Projection = new EmbeddingProjectRequest
                {
                    CurrentState = current.ToProtobuf(),
                    Event = new Events.Contracts.UncommittedEvent
                    {
                        EventType= @event.Type.ToProtobuf(),
                        Content = @event.Content,
                        EventSourceId = @event.EventSource.Value,
                        Public = @event.Public,
                    }
                }
            };

        /// <inheritdoc/>
        public EmbeddingRequest Create(EmbeddingCurrentState current, ProjectionState desiredState)
            => new()
            {
                Compare = new EmbeddingCompareRequest
                {
                    ProjectionState = current.ToProtobuf(),
                    EntityState = desiredState
                }
            };

        /// <inheritdoc/>
        public EmbeddingRequest Create(EmbeddingCurrentState current)
            => new()
            {
                Delete = new EmbeddingDeleteRequest
                {
                    ProjectionState = current.ToProtobuf()
                }
            };

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(ProjectionCurrentState current, UncommittedEvent @event)
            => Try<EmbeddingRequest>.Do(() => Create(current, @event));

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current, ProjectionState desiredState)
            => Try<EmbeddingRequest>.Do(() => Create(current, desiredState));

        /// <inheritdoc/>
        public Try<EmbeddingRequest> TryCreate(EmbeddingCurrentState current)
            => Try<EmbeddingRequest>.Do(() => Create(current));
    }
}
