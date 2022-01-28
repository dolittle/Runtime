// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_removing;

public class and_deletion_is_acknowledged : given.a_projection_copy_store_and_a_projection
{
    static bool result;

    Because of = () => result = copy_store.TryRemove(projection, projection_key, cancellation_token).GetAwaiter().GetResult();

    It should_get_the_correct_collection = () => database.Verify(_ => _.GetCollection<BsonDocument>(collection_name, Moq.It.IsAny<MongoCollectionSettings>()));
    It should_delete_the_document_with_the_correct_filter = () => collection.Verify(_ => _.DeleteOneAsync(
        IsFilter(document => document["_id"].AsString == projection_key.Value),
        cancellation_token), Times.Once);
    It should_return_true = () => result.ShouldBeTrue();
}