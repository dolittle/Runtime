// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_replacing;

public class and_collection_name_is_not_set : given.a_projection_copy_store_and_a_projection
{
    Establish context = () =>
    {
        projection = projection with
        {
            Copies = projection.Copies with
            {
                MongoDB = projection.Copies.MongoDB with
                {
                    Collection = CollectionName.NotSet,
                }
            }
        };
    };

    static Exception exception;

    Because of = () => exception = Catch.Exception(() => copy_store.TryReplace(projection, projection_key, projection_state, cancellation_token).GetAwaiter().GetResult());

    It should_fail = () => exception.ShouldBeOfExactType<ProjectionShouldNotBeCopiedToMongoDB>();
    It should_not_have_replaced_the_document = () => collection.Verify(_ => _.ReplaceOneAsync(Moq.It.IsAny<FilterDefinition<BsonDocument>>(), Moq.It.IsAny<BsonDocument>(), Moq.It.IsAny<ReplaceOptions>(), Moq.It.IsAny<CancellationToken>()), Times.Never);
}