// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using static Moq.Times;

namespace Dolittle.Runtime.Embeddings.Store.Services.for_EmbeddingsService;

public class when_getting_keys : given.the_service
{
    static IEnumerable<ProjectionKey> stored_keys;
    Establish context = () =>
    {
        stored_keys = new ProjectionKey[]
        {
            key,
            "some key",
            "some different key",


        };
        embedding_store
            .Setup(_ => _.TryGetKeys(embedding, cancellation_token))
            .Returns(Task.FromResult(Try<IEnumerable<ProjectionKey>>.Succeeded(stored_keys)));
    };

    static Try<IEnumerable<ProjectionKey>> result;

    Because of = () => result = service.TryGetKeys(embedding, execution_context, cancellation_token).GetAwaiter().GetResult();

    It should_create_the_execution_context = () => execution_context_creator.Verify(_ => _.TryCreateUsing(execution_context));
    It should_call_the_embedding_store = () => embedding_store.Verify(_ => _.TryGetKeys(embedding, cancellation_token), Once);
    It should_return_successfull_result = () => result.Success.ShouldBeTrue();
    It should_return_the_current_state = () => result.Result.ShouldContainOnly(stored_keys);
}