// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Aggregates;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Projections.Store.State;
using Dolittle.Runtime.Rudimentary;
using FluentAssertions;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_replacing;

public class and_it_succeeds : given.all_dependencies
{
    static EmbeddingId id;
    static ProjectionKey key;
    static AggregateRootVersion version;
    static ProjectionState state;
    Establish context = () =>
    {
        id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
        key = new ProjectionKey("test_key");
        version = AggregateRootVersion.Initial;
        state = new ProjectionState("im an emojiless state");

        states
            .Setup(_ => _.TryReplace(id, key, Moq.It.IsAny<EmbeddingState>(), Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<bool>.Succeeded(true)));
    };

    static Try result;

    Because of = () => result = store.TryReplace(id, key, version, state, CancellationToken.None).GetAwaiter().GetResult();

    It should_succeed = () => result.Success.Should().BeTrue();
}