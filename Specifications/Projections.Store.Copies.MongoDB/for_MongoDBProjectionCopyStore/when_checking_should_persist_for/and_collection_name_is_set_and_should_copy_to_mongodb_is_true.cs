// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_checking_should_persist_for;

public class and_collection_name_is_set_and_should_copy_to_mongodb_is_true : given.a_projection_copy_store_and_a_projection
{
    static bool result;

    Because of = () => result = copy_store.ShouldPersistFor(projection);

    It should_return_true = () => result.ShouldBeTrue();
}