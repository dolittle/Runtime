// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Processing
{

    /// <summary>
    /// Represents an implementation of <see cref="IEmbedding" />.
    /// </summary>
    public class Embedding : IEmbedding
    {
        readonly EmbeddingId _identifier;
        readonly IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> _dispatcher;
        readonly IEmbeddingRequestFactory _requestFactory;

        public Embedding(
            EmbeddingId identifier,
            IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> dispatcher,
            IEmbeddingRequestFactory requestFactory)
        {
            _identifier = identifier;
            _dispatcher = dispatcher;
            _requestFactory = requestFactory;
        }

        /// <inheritdoc/> 
        public Task<IProjectionResult> Project(ProjectionCurrentState state, UncommittedEvent @event, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/> 
        public Task<Try<UncommittedEvents>> TryCompare(EmbeddingCurrentState currentState, ProjectionState desiredState, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/> 
        public Task<Try<UncommittedEvents>> TryDelete(EmbeddingCurrentState currentState, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
