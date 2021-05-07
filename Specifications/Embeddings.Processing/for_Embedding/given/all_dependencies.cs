// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using Dolittle.Runtime.Embeddings.Contracts;
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

        Establish context = () =>
        {
            cancellation = CancellationToken.None;
            dispatcher = new Mock<IReverseCallDispatcher<EmbeddingClientToRuntimeMessage, EmbeddingRuntimeToClientMessage, EmbeddingRegistrationRequest, EmbeddingRegistrationResponse, EmbeddingRequest, EmbeddingResponse>>(MockBehavior.Strict);
            var embeddingId = "76f59adf-5a48-442c-b5d1-0a78a0a51d80";
            request_factory = new Mock<IEmbeddingRequestFactory>();
            embedding_request = new EmbeddingRequest();
            embedding = new Embedding(embeddingId, dispatcher.Object, request_factory.Object);
        };
    }
}
