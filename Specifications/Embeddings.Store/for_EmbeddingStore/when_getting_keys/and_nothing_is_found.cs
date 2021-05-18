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
    public class and_nothing_is_found : given.all_dependencies
    {

        static EmbeddingId id;

        Establish context = () =>
        {
            id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));

            states
                .Setup(_ => _.TryGetAll(id, Moq.It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(Try<IEnumerable<(EmbeddingState, ProjectionKey)>>.Failed()));
        };

        static Try<IEnumerable<ProjectionKey>> result;

        Because of = () => result = store.TryGetKeys(id, CancellationToken.None).Result;

        It should_fail = () => result.Success.ShouldBeFalse();
        It should_return_a_failure = () => result.HasException.ShouldBeTrue();
    }
}
