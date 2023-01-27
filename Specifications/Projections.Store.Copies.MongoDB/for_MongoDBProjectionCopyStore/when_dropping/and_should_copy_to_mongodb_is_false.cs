// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using FluentAssertions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_dropping;

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

    static Exception exception;

    Because of = () => exception = Catch.Exception(() => copy_store.TryDrop(projection, cancellation_token).GetAwaiter().GetResult());

    It should_fail = () => exception.Should().BeOfType<ProjectionShouldNotBeCopiedToMongoDB>();
    It should_not_have_dropped_the_collection = () => database.Verify(_ => _.DropCollectionAsync(collection_name, cancellation_token), Times.Never);
}