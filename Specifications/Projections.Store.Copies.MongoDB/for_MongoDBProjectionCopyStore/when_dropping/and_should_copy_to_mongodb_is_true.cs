// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_dropping;

public class and_should_copy_to_mongodb_is_true : given.a_projection_copy_store_and_a_projection
{
    static bool result;

    Because of = () => result = copy_store.TryDrop(projection, cancellation_token).GetAwaiter().GetResult();

    It should_return_true = () => result.ShouldBeTrue();
    It should_have_dropped_the_collection = () => database.Verify(_ => _.DropCollectionAsync(collection_name, cancellation_token), Times.Once);
}