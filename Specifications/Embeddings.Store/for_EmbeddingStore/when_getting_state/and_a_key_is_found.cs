// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_getting_state
{
    public class and_a_key_is_found : given.all_dependencies
    {

        static EmbeddingId id;
        static ProjectionKey key;
        static EmbeddingState persisted_state;

        Establish context = () =>
        {
            id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
            key = new ProjectionKey("test_key");

            persisted_state = new EmbeddingState("persisted_state", 1);

            states
                .Setup(_ => _.TryGet(id, key, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingState>.Succeeded(persisted_state)));
        };

        static Try<EmbeddingCurrentState> result;

        Because of = () => result = store.TryGet(id, key, CancellationToken.None).Result;

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_get_the_state = () => result.Result.State.Value.ShouldEqual(persisted_state.State);
        It should_get_the_key = () => result.Result.Key.ShouldEqual(key);
        It should_have_the_same_aggregate_version = () => result.Result.Version.ShouldEqual(persisted_state.Version);
        It should_get_a_persisted_state = () => result.Result.Type.ShouldEqual(ProjectionCurrentStateType.Persisted);

    }
}
