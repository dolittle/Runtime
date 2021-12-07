// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Processing;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Protobuf;
using Machine.Specifications;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc.for_EmbeddingStoreGrpcService.when_getting.one;

public class and_it_succeeded : given.the_service
{
    static Contracts.GetOneRequest request;

    Establish context = () =>
    {
        request = new Contracts.GetOneRequest()
        {
            EmbeddingId = embedding.ToProtobuf(),
            Key = key,
            CallContext = new Dolittle.Services.Contracts.CallRequestContext
            {
                ExecutionContext = execution_context.ToProtobuf(),
                HeadId = Guid.Parse("3acf909e-f696-4bef-a93c-21f995117707").ToProtobuf()
            }
        };
        embeddings_service
            .Setup(_ => _.TryGetOne(embedding, key, execution_context, cancellation_token))
            .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(a_current_state));
    };

    static Contracts.GetOneResponse result;

    Because of = () => result = grpc_service.GetOne(request, call_context).GetAwaiter().GetResult();

    It should_call_the_service = () => embeddings_service.Verify(_ => _.TryGetOne(embedding, key, execution_context, cancellation_token), Once);
    It should_not_have_a_failure = () => result.Failure.ShouldBeNull();
    It should_return_the_current_state = () => result.State.ShouldEqual(a_current_state.ToProtobuf());
}