// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_getting_keys
{
    public class and_a_key_is_not_found : given.all_dependencies
    {

        static EmbeddingId id;
        static ProjectionKey key;
        static ProjectionState initial_state;
        static EmbeddingDefinition definition;

        Establish context = () =>
        {
            id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
            key = new ProjectionKey("test_key");

            initial_state = new ProjectionState("being 😑 an ⬅👴 initial 💰 state 🙈 do 👏 be 👨🅱");
            var events = new List<ProjectionEventSelector>();
            definition = new EmbeddingDefinition(id, events, initial_state);

            states
                .Setup(_ => _.TryGet(id, key, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingState>.Failed()));

            definitions
                .Setup(_ => _.TryGet(id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(definition)));
        };

        static Try<EmbeddingCurrentState> result;

        Because of = () => result = store.TryGet(id, key, CancellationToken.None).Result;

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_get_a_state = () => result.Result.State.ShouldEqual(initial_state);
        It should_get_the_key = () => result.Result.Key.ShouldEqual(key);
        It should_have_an_aggregate_version_of_0 = () => result.Result.Version.Value.ShouldEqual((ulong)0);
        It should_get_an_initial_state = () => result.Result.Type.ShouldEqual(ProjectionCurrentStateType.CreatedFromInitialState);
    }
}
