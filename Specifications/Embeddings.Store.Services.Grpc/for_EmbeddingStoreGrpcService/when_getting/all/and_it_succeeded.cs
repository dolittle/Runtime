// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Processing;
using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Protobuf;
using FluentAssertions;
using Machine.Specifications;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.Grpc.for_EmbeddingStoreGrpcService.when_getting.all;

public class and_it_succeeded : given.the_service
{
    static Contracts.GetAllRequest request;
    static IEnumerable<EmbeddingCurrentState> stored_states;
    Establish context = () =>
    {
        request = new Contracts.GetAllRequest()
        {
            EmbeddingId = embedding.ToProtobuf(),
            CallContext = new Dolittle.Services.Contracts.CallRequestContext
            {
                ExecutionContext = execution_context.ToProtobuf(),
                HeadId = Guid.Parse("3acf909e-f696-4bef-a93c-21f995117707").ToProtobuf()
            }
        };
        stored_states = new[]
        {
            a_current_state,
            a_current_state with { State = "some other state" },
            a_current_state with { State = "weird state" }
        };
        embeddings_service
            .Setup(_ => _.TryGetAll(embedding, execution_context, cancellation_token))
            .Returns(Task.FromResult(Try<IEnumerable<EmbeddingCurrentState>>.Succeeded(stored_states)));
    };

    static Contracts.GetAllResponse result;

    Because of = () => result = grpc_service.GetAll(request, call_context).GetAwaiter().GetResult();

    It should_call_the_service = () => embeddings_service.Verify(_ => _.TryGetAll(embedding, execution_context, cancellation_token), Once);
    It should_not_have_a_failure = () => result.Failure.Should().BeNull();
    It should_return_the_correct_states = () => result.States.ShouldContainOnly(stored_states.Select(_ => _.ToProtobuf()));
}