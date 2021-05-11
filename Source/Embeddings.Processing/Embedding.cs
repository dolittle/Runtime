// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Events.Processing.Projections;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Services;

namespace Dolittle.Runtime.Embeddings.Processing
{

    /// <summary>
    /// Represents an implementation of <see cref="IEmbedding" />.
    /// </summary>
    public class Embedding : IEmbedding
    {
        readonly EmbeddingId _embeddingId;
        readonly IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> _dispatcher;
        readonly IEmbeddingRequestFactory _requestFactory;

        /// <summary>
        /// Initializes an instance of the <see cref="Embedding" /> class.
        /// </summary>
        /// <param name="embeddingId">The <see cref="EmbeddingId" />.</param>
        /// <param name="dispatcher">The <see cref="IReverseCallDispatcher{TClientMessage, TServerMessage, TConnectArguments, TConnectResponse, TRequest, TResponse}" />.</param>
        /// <param name="requestFactory">The <see cref="IEmbeddingRequestFactory" />.</param>
        public Embedding(
            EmbeddingId embeddingId,
            IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse> dispatcher,
            IEmbeddingRequestFactory requestFactory)
        {
            _embeddingId = embeddingId;
            _dispatcher = dispatcher;
            _requestFactory = requestFactory;
        }

        /// <inheritdoc/> 
        public async Task<IProjectionResult> Project(ProjectionCurrentState state, UncommittedEvent @event, CancellationToken cancellationToken)
            => HandleProjectionResponse(
                await _dispatcher.Call(
                    _requestFactory.Create(state, @event),
                    cancellationToken).ConfigureAwait(false));

        /// <inheritdoc/> 
        public Task<Try<UncommittedEvents>> TryCompare(EmbeddingCurrentState currentState, ProjectionState desiredState, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/> 
        public async Task<Try<UncommittedEvents>> TryDelete(EmbeddingCurrentState currentState, CancellationToken cancellationToken)
        {
            try
            {
                var request = _requestFactory.Create(currentState);
                var response = await _dispatcher.Call(request, cancellationToken).ConfigureAwait(false);
                if (IsFailureResponse(response))
                {
                    return new FailedDeleteEmbedding(_embeddingId, GetFailureReason(response));
                }

                return response.ResponseCase switch
                {
                    EmbeddingResponse.ResponseOneofCase.Delete => ToUncommittedEvents(response.Delete.Events),
                    _ => new UnexpectedEmbeddingResponse(_embeddingId, response.ResponseCase)
                };
            }
            catch (Exception ex)
            {
                return ex;
            }
        }

        IProjectionResult HandleProjectionResponse(EmbeddingResponse response)
        {
            if (IsFailureResponse(response))
            {
                return new ProjectionFailedResult(GetFailureReason(response));
            }
            return response.ResponseCase switch
            {
                EmbeddingResponse.ResponseOneofCase.ProjectionDelete => new ProjectionDeleteResult(),
                EmbeddingResponse.ResponseOneofCase.ProjectionReplace => new ProjectionReplaceResult(response.ProjectionReplace.State),
                EmbeddingResponse.ResponseOneofCase.Compare
                    => new ProjectionFailedResult($"Unexpected response case {response.ResponseCase}"),
                EmbeddingResponse.ResponseOneofCase.Delete
                    => new ProjectionFailedResult($"Unexpected response case {response.ResponseCase}"),
                _ => new ProjectionFailedResult($"Unexpected response case {response.ResponseCase}")
            };
        }

        bool IsFailureResponse(EmbeddingResponse response)
            => response.Failure is not null || response.ProcessorFailure is not null;

        string GetFailureReason(EmbeddingResponse response) => response.ProcessorFailure?.Reason ?? response.Failure.Reason;

        UncommittedEvents ToUncommittedEvents(IEnumerable<Events.Contracts.UncommittedEvent> events)
            => new(
                new ReadOnlyCollection<UncommittedEvent>(events.Select(_ =>
                    new UncommittedEvent
                    (_.EventSourceId.ToGuid(),
                    new Artifact(_.Artifact.Id.ToGuid(),
                    _.Artifact.Generation),
                    _.Public,
                    _.Content)).ToList()));
    }
}
