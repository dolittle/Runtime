// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_checking_should_persist_for;

public class and_should_copy_to_mongodb_is_false : given.a_projection_copy_store_and_a_projection
{
    Establish context = () =>
    {
        projection = projection with
        {
            Copies = projection.Copies with
            {
                MongoDB = projection.Copies.MongoDB with
                {
                    ShouldCopyToMongoDB = false,
                }
            }
        };
    };

    static bool result;

    Because of = () => result = copy_store.ShouldPersistFor(projection);

    It should_return_false = () => result.Should().BeFalse();
}