// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionRenamer.when_renaming;

#pragma warning disable CS0618

public class without_conversions : given.a_renamer_and_inputs
{
    Establish context = () =>
    {
        document_to_rename = new BsonDocument(new[]
        {
            new BsonElement("some_int", new BsonInt32(42)),
            new BsonElement("some_string", new BsonString("hello world")),
            new BsonElement("some_date", new BsonDateTime(DateTime.Parse("1905-11-21T00:00:00.000Z"))),
            new BsonElement("some_guid", new BsonBinaryData(Guid.Parse("f91fe21f-5a35-46ff-aa43-52500ac79fa7"))),
            new BsonElement("some_array", new BsonArray()
            {
                new BsonString("other string"),
                new BsonInt32(1337),
                new BsonDateTime(DateTime.Parse("2013-01-01T00:00:00.000Z")),
                new BsonBinaryData(Guid.Parse("528a7010-cac7-4505-b7e2-e150fb8f5d7c")),
                new BsonDocument(new[]
                {
                    new BsonElement("nested_int", new BsonInt32(13)),
                    new BsonElement("nested_array", new BsonArray()
                    {
                        new BsonDouble(3.1415962),
                        new BsonDouble(2.718281828),
                    }),
                }),
            }),
        });
    };

    It should_return_a_document_that_is_equal = () => result.ShouldEqual(document_to_rename);
    It should_not_return_the_same_document = () => result.ShouldNotBeTheSameAs(document_to_rename);
}