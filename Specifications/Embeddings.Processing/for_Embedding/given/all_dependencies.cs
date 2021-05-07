// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Services;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Embeddings.Processing.for_Embedding.given
{
    public class all_dependencies
    {
        protected static Mock<IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>> dispatcher;
        protected static IEmbedding embedding;
        protected static CancellationToken cancellation;
        protected static Mock<IEmbeddingRequestFactory> request_factory;
        protected static EmbeddingRequest embedding_request;
        protected static UncommittedEvent @event;
        protected static ProjectionCurrentState current_state;

        Establish context = () =>
        {
            cancellation = CancellationToken.None;
            dispatcher = new Mock<IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>>(MockBehavior.Strict);
            var embeddingId = Guid.NewGuid();
            request_factory = new Mock<IEmbeddingRequestFactory>();
            embedding_request = new EmbeddingRequest();
            embedding = new Embedding(embeddingId, dispatcher.Object, request_factory.Object);
            @event = new UncommittedEvent(
                Guid.Parse("1a477367-3404-45e6-a2af-6cbf19693b56"),
                new Artifact(Guid.Parse("fe570d85-2619-49e4-bc72-9a8b2b2f149d"), ArtifactGeneration.First),
                true,
                "beautiful 😍 event to 🙅 be tested 🤓🤓")
            );

            current_state = new ProjectionCurrentState(
                ProjectionCurrentStateType.Persisted,
                "projectionState",
                "projectionKey");

        };
    }
}
