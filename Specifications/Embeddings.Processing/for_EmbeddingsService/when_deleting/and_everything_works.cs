// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Domain.Tenancy;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Embeddings.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;

namespace Dolittle.Runtime.Embeddings.Processing.for_EmbeddingsService.when_deleting;

public class and_everything_works : given.all_dependencies
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
                _processor = embedding_processor.Object;
                return true;
            }));

        embedding_processor
            .Setup(_ => _.Delete(key, execution_context, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try.Succeeded));
    };

    static DeleteResponse result;

    Because of = () =>
    {
        result = embedding_service.Delete(request, call_context).GetAwaiter().GetResult();
    };

    It should_delete_once = () => embedding_processor.Verify(_ => _.Delete(key, execution_context, Moq.It.IsAny<CancellationToken>()), Moq.Times.Once);
    It should_not_do_anything_else_with_processor = () => embedding_processor.VerifyNoOtherCalls();
    It should_not_have_a_failure = () => result.Failure.ShouldBeNull();
}