// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Dolittle.Runtime.Projections.Store.State;
using Machine.Specifications;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Moq;
using It = Moq.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_MongoDBProjectionCopyStore.given;

public class a_projection_copy_store_and_a_projection : a_projection
{
    protected static Mock<IMongoCollection<BsonDocument>> collection;
    protected static Mock<IMongoDatabase> database;
    protected static Mock<IProjectionCopiesStorage> storage;
    
    protected static BsonDocument converted_bson_document;
    protected static Mock<IProjectionConverter> converter;

    protected static MongoDBProjectionCopyStore copy_store;

    protected static CancellationToken cancellation_token;

    Establish context = () =>
    {
        collection = new Mock<IMongoCollection<BsonDocument>>();
        collection
            .Setup(_ => _.ReplaceOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), It.IsAny<BsonDocument>(), It.IsAny<ReplaceOptions>(), cancellation_token))
            .ReturnsAsync(new ReplaceOneResult.Acknowledged(1, 1, BsonObjectId.Empty));
        collection
            .Setup(_ => _.DeleteOneAsync(It.IsAny<FilterDefinition<BsonDocument>>(), cancellation_token))
            .ReturnsAsync(new DeleteResult.Acknowledged(1));

        database = new Mock<IMongoDatabase>();
        database
            .Setup(_ => _.GetCollection<BsonDocument>(collection_name, null))
            .Returns(collection.Object);
        
        storage = new Mock<IProjectionCopiesStorage>();
        storage
            .SetupGet(_ => _.Database)
            .Returns(database.Object);

        converted_bson_document = new BsonDocument();

        converter = new Mock<IProjectionConverter>();
        converter
            .Setup(_ => _.Convert(It.IsAny<ProjectionState>(), It.IsAny<PropertyConversion[]>()))
            .Returns(converted_bson_document);

        copy_store = new MongoDBProjectionCopyStore(storage.Object, converter.Object);
        
        cancellation_token = CancellationToken.None;
    };

    protected static FilterDefinition<BsonDocument> IsFilter(Func<BsonDocument, bool> matcher)
        => Match.Create<FilterDefinition<BsonDocument>>(definition =>
        {
            var document = definition.Render(new BsonDocumentSerializer(), BsonSerializer.SerializerRegistry);
            return matcher(document);
        });
}