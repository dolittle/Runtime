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
        readonly IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> _dispatcher;
        readonly IEmbeddingRequestFactory _requestFactory;

        /// <summary>
        /// Initializes an instance of the <see cref="Embedding" /> class.
        /// </summary>
        /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="requestFactory">The <see cref="IEmbeddingRequestFactory" />.</param>
        public Embedding(
            IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> dispatcher,
            IEmbeddingRequestFactory requestFactory)
        {
            _dispatcher = dispatcher;
            _requestFactory = requestFactory;
        }

        /// <inheritdoc/> 
        public async Task<IProjectionResult> Project(ProjectionCurrentState state, UncommittedEvent @event, CancellationToken cancellationToken)
            => HandleResponse(
                await _dispatcher.Call(
                    _requestFactory.Create(state, @event),
                    cancellationToken).ConfigureAwait(false));

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

        IProjectionResult HandleResponse(EmbeddingResponse response)
        {
            if (IsFailureResponse(response))
            {
                return response.FailureCase is EmbeddingResponse.FailureOneofCase.ProcessorFailure
                    ? new ProjectionFailedResult(response.ProcessorFailure.Reason)
                    : new ProjectionFailedResult(response.Failure.Reason);
            }
            return response.ResponseCase switch
            {
                EmbeddingResponse.ResponseOneofCase.Compare
                    => new ProjectionFailedResult($"Wrong response case {nameof(EmbeddingResponse.ResponseOneofCase.Compare)}"),
                EmbeddingResponse.ResponseOneofCase.Delete
                    => new ProjectionFailedResult($"Wrong response case {nameof(EmbeddingResponse.ResponseOneofCase.Delete)}"),
                EmbeddingResponse.ResponseOneofCase.ProjectionDelete => new ProjectionDeleteResult(),
                EmbeddingResponse.ResponseOneofCase.ProjectionReplace => new ProjectionReplaceResult(response.ProjectionReplace.State),
                _ => new ProjectionFailedResult($"Unexpected response case {response.ResponseCase}")
            };
        }

        bool IsFailureResponse(EmbeddingResponse response)
            => response.Failure is not null || response.ProcessorFailure is not null;
    }
}
