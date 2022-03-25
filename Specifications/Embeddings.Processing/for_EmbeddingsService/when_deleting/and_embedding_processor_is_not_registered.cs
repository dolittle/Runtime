// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.


using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_deleting;

public class and_embedding_processor_is_not_registered : given.all_dependencies
{
    static IEmbeddingProcessor processor;
    static DeleteRequest request;
    static ProjectionKey key;

    Establish context = () =>
    {
        key = "some key";
        request = new DeleteRequest()
        {
            CallContext = call_request_context,
            EmbeddingId = embedding_id.ToProtobuf(),
            Key = key
        };
        embedding_processors
            .Setup(_ => _.TryGetEmbeddingProcessorFor(execution_context.Tenant, embedding_id, out processor))
            .Returns(new given.TryGetEmbeddingProcessorForReturns((TenantId _tenant, EmbeddingId _embedding, out IEmbeddingProcessor _processor) =>
            {
                _processor = null;
                return false;
            }));
    };

    static DeleteResponse result;

    Because of = () =>
    {
        result = embedding_service.Delete(request, call_context).GetAwaiter().GetResult();
    };
    It should_have_a_failure = () => result.Failure.ShouldNotBeNull();
    It should_have_a_failure_with_correct_failure_id = () => result.Failure.Id.ShouldEqual(EmbeddingFailures.NoEmbeddingRegisteredForTenant.ToProtobuf());
}