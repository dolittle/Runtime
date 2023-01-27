// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using FluentAssertions;
using Machine.Specifications;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.when_replacing;

public class and_replacing_is_not_acknowledged : given.a_projection_copy_store_and_a_projection
{
    Establish context = () =>
    {
        collection
            .Setup(_ => _.ReplaceOneAsync(Moq.It.IsAny<FilterDefinition<BsonDocument>>(), Moq.It.IsAny<BsonDocument>(), Moq.It.IsAny<ReplaceOptions>(), cancellation_token))
            .ReturnsAsync(ReplaceOneResult.Unacknowledged.Instance);
    };

    static bool result;

    Because of = () => result = copy_store.TryReplace(projection, projection_key, projection_state, cancellation_token).GetAwaiter().GetResult();

    It should_get_the_correct_collection = () => database.Verify(_ => _.GetCollection<BsonDocument>(collection_name, Moq.It.IsAny<MongoCollectionSettings>()));
    It should_convert_the_projection = () => converter.Verify(_ => _.Convert(projection_state, Moq.It.IsAny<PropertyConversion[]>()));
    It should_replace_the_document_with_the_correct_filter_and_document_and_options = () => collection.Verify(_ => _.ReplaceOneAsync(
        IsFilter(document => document["_dolittle_projection_key"].AsString == projection_key.Value),
        Moq.It.Is<BsonDocument>(document => document == converted_bson_document && document["_dolittle_projection_key"].AsString == projection_key.Value),
        Moq.It.Is<ReplaceOptions>(options => options.IsUpsert == true),
        cancellation_token), Times.Once);
    It should_return_false = () => result.Should().BeFalse();
}