// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using static Moq.It;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.for_EmbeddingsService;

public class when_getting_all_states : given.the_service
{
    static IEnumerable<EmbeddingCurrentState> stored_states;
    Establish context = () =>
    {
        stored_states = new[]
        {
            a_current_state,
            a_current_state with { State = "some other state" },
            a_current_state with { State = "weird state" }
        };
        embedding_store
            .Setup(_ => _.TryGetAll(embedding, cancellation_token))
            .Returns(Task.FromResult(Try<IEnumerable<EmbeddingCurrentState>>.Succeeded(stored_states)));
    };

    static Try<IEnumerable<EmbeddingCurrentState>> result;

    Because of = () => result = service.TryGetAll(embedding, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_create_the_execution_context = () => execution_context_creator.Verify(_ => _.TryCreateUsing(execution_context));
    It should_call_the_embedding_store = () => embedding_store.Verify(_ => _.TryGetAll(embedding, cancellation_token), Once);
    It should_return_successfull_result = () => result.Success.ShouldBeTrue();
    It should_return_the_current_state = () => result.Result.ShouldContainOnly(stored_states);
}