// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.for_EmbeddingsService;

public class when_getting_one_state : given.the_service
{
    Establish context = () =>
    {
        embedding_store
            .Setup(_ => _.TryGet(embedding, key, cancellation_token))
            .Returns(Task.FromResult<Try<EmbeddingCurrentState>>(a_current_state));
    };

    static Try<EmbeddingCurrentState> result;

    Because of = () => result = service.TryGetOne(embedding, key, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_set_the_execution_context = () => execution_context_manager.Verify(_ => _.CurrentFor(execution_context, IsAny<string>(), IsAny<int>(), IsAny<string>()));
    It should_call_the_embedding_store = () => embedding_store.Verify(_ => _.TryGet(embedding, key, cancellation_token), Once);
    It should_return_successfull_result = () => result.Success.ShouldBeTrue();
    It should_return_the_current_state = () => result.Result.ShouldEqual(a_current_state);
}