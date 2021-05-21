// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Embeddings.Store.Definition;
using Dolittle.Runtime.Embeddings.Store.State;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_getting_state
{
    public class and_a_definition_is_not_found : given.all_dependencies
    {

        static EmbeddingId id;
        static ProjectionKey key;
        Establish context = () =>
        {
            id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
            key = new ProjectionKey("test_key");

            states
                .Setup(_ => _.TryGet(id, key, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingState>.Failed(new EmbeddingStateDoesNotExist(id, key))));

            definitions
                .Setup(_ => _.TryGet(id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<EmbeddingDefinition>.Failed(new Exception())));
        };

        static Try<EmbeddingCurrentState> result;

        Because of = () => result = store.TryGet(id, key, CancellationToken.None).GetAwaiter().GetResult();

        It should_fail = () => result.Success.ShouldBeFalse();

        It should_return_a_failure = () => result.Exception.ShouldNotBeNull();
    }
}
