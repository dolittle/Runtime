// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Artifacts;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_getting_state;

public class and_its_marked_as_removed : given.all_dependencies
{

    static EmbeddingId id;
    static ProjectionKey key;
    static EmbeddingState persisted_state;
    static ProjectionState initial_state;

    Establish context = () =>
    {
        id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
        key = new ProjectionKey("test_key");

        persisted_state = new EmbeddingState("persisted_state", 10, true);

        initial_state = new ProjectionState("im 😂 an 😒🐖 initial 💇 state to 💦📬 a 💰 removed ❌🗑 embedding");
        var events = new List<Artifact>();
        var definition = new EmbeddingDefinition(id, events, initial_state);

        states
            .Setup(_ => _.TryGet(id, key, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<EmbeddingState>.Succeeded(persisted_state)));

        definitions
            .Setup(_ => _.TryGet(id, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<EmbeddingDefinition>.Succeeded(definition)));
    };

    static Try<EmbeddingCurrentState> result;

    Because of = () => result = store.TryGet(id, key, CancellationToken.None).GetAwaiter().GetResult();

    It should_succeed = () => result.Success.ShouldBeTrue();
    It should_get_the_key = () => result.Result.Key.ShouldEqual(key);
    It should_get_the_state_from_the_definition = () => result.Result.State.Value.ShouldEqual(initial_state.Value);
    It should_get_the_aggregate_version = () => result.Result.Version.ShouldEqual(persisted_state.Version);
    It should_get_an_initial_state_type = () => result.Result.Type.ShouldEqual(EmbeddingCurrentStateType.CreatedFromInitialState);

}