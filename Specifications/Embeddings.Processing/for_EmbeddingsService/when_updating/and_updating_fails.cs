// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_updating;

public class and_updating_fails : given.all_dependencies
{
    static IEmbeddingProcessor processor;
    static UpdateRequest request;
    static ProjectionKey key;
    static ProjectionState state;

    Establish context = () =>
    {
        key = "some key";
        state = "some state";
        request = new UpdateRequest
        {
            CallContext = call_request_context,
            EmbeddingId = embedding_id.ToProtobuf(),
            Key = key,
            State = state
        };
        embedding_processors
            .Setup(_ => _.TryGetEmbeddingProcessorFor(execution_context.Tenant, embedding_id, out processor))
            .Returns(new given.TryGetEmbeddingProcessorForReturns((TenantId _tenant, EmbeddingId _embedding, out IEmbeddingProcessor _processor) =>
            {
                _processor = embedding_processor.Object;
                return true;
            }));

        embedding_processor
            .Setup(_ => _.Update(key, state, execution_context, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<ProjectionState>.Failed(new System.Exception())));
    };

    static UpdateResponse result;

    Because of = () =>
    {
        result = embedding_service.Update(request, call_context).GetAwaiter().GetResult();
    };
    It should_have_a_failure = () => result.Failure.ShouldNotBeNull();
    It should_have_a_failure_with_correct_failure_id = () => result.Failure.Id.ShouldEqual(EmbeddingFailures.FailedToUpdateEmbedding.ToProtobuf());
}