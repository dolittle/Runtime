// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Projections.Store;
using Dolittle.Runtime.Rudimentary;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Embeddings.Store.for_EmbeddingStore.when_removing;

public class and_it_fails : given.all_dependencies
{
    static EmbeddingId id;
    static ProjectionKey key;
    static AggregateRootVersion version;
    static Exception exception;
    Establish context = () =>
    {
        id = new EmbeddingId(Guid.Parse("091e7458-e1d2-4b21-b134-bf5a42ce1ef5"));
        key = new ProjectionKey("test_key");
        version = AggregateRootVersion.Initial;
        exception = new Exception();

        states
            .Setup(_ => _.TryMarkAsRemove(id, key, version, Moq.It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(Try<bool>.Failed(exception)));
    };

    static Try result;

    Because of = () => result = store.TryRemove(id, key, version, CancellationToken.None).GetAwaiter().GetResult();

    It should_fail = () => result.Success.ShouldBeFalse();
    It should_have_the_correct_exception = () => result.Exception.ShouldEqual(exception);
}