// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Contracts;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Protobuf;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Services.Contracts;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc.for_EmbeddingStoreGrpcService.when_getting.keys
{
    public class and_there_are_no_keys : given.the_service
    {
        static Contracts.GetKeysRequest request;
        static IEnumerable<ProjectionKey> stored_keys;
        Establish context = () =>
        {
            request = new GetKeysRequest
            {
                EmbeddingId = embedding.ToProtobuf(),
                CallContext = new CallRequestContext
                {
                    ExecutionContext = execution_context.ToProtobuf()
                }
            };
            stored_keys = Enumerable.Empty<ProjectionKey>();
            embeddings_service
                .Setup(_ => _.TryGetKeys(embedding, execution_context, cancellation_token))
                .Returns(Task.FromResult(Try<IEnumerable<ProjectionKey>>.Succeeded(stored_keys)));
        };

        static Contracts.GetKeysResponse result;

        Because of = () => result = grpc_service.GetKeys(request, call_context).GetAwaiter().GetResult();

        It should_call_the_service = () => embeddings_service.Verify(_ => _.TryGetKeys(embedding, execution_context, cancellation_token), Once);
        It should_not_have_a_failure = () => result.Failure.ShouldBeNull();
        It should_return_an_empty_result = () => result.Keys.ShouldBeEmpty();
    }
}
