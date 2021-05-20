// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_getting_all_states
{
    public class and_all_are_found : given.all_dependencies
    {
        static EmbeddingId id;
        static List<(EmbeddingState, ProjectionKey)> persisted_states;

        Establish context = () =>
        {
            id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));

            persisted_states = new List<(EmbeddingState, ProjectionKey)>
            {
                (new EmbeddingState("persisted_state 1", 1, false), "first"),
                (new EmbeddingState("persisted_state 2", 1, false), "second"),
                (new EmbeddingState("persisted_state 🌲", 1, false), "third"),
                (new EmbeddingState("persisted_state removed", 1, true), "fourth"),
                (new EmbeddingState("persisted_state five", 1, false), "fifth"),
            };

            states
                .Setup(_ => _.TryGetAll(id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(
                    Try<IEnumerable<(EmbeddingState, ProjectionKey)>>.Succeeded(persisted_states)));
        };

        static Try<IEnumerable<EmbeddingCurrentState>> result;

        Because of = () => result = store.TryGetAll(id, true, CancellationToken.None).Result;

        It should_succeed = () => result.Success.ShouldBeTrue();
        It should_get_the_current_states = () =>
            result.Result.ShouldEachConformTo(current_state =>
                persisted_states.Contains(
                    new Tuple<EmbeddingState, ProjectionKey>(
                        new EmbeddingState(
                            current_state.State.Value,
                            current_state.Version,
                            current_state.Type == EmbeddingCurrentStateType.Deleted),
                        current_state.Key)
                    .ToValueTuple()));
    }
}
