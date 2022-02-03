// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.without_conversions;

#pragma warning disable CS0618

public class objects : given.a_converter_and_inputs
{
    Establish context = () => state_to_convert = @"
        {
            ""first_object"":
            {
                ""some_string"": ""hello world"",
                ""some_int"": 42
            },
            ""second_object"":
            {
                ""some_bool"": true,
                ""some_date"": ""2002-02-02T02:02:02.002Z""
            }
        }
    ";

    It should_have_the_correct_first_object = () => result["first_object"].ShouldEqual(new BsonDocument(new []
    {
        new BsonElement("some_string", new BsonString("hello world")),
        new BsonElement("some_int", new BsonInt32(42)),
    }));
    It should_have_the_correct_second_object = () => result["second_object"].ShouldEqual(new BsonDocument(new []
    {
        new BsonElement("some_bool", new BsonBoolean(true)),
        new BsonElement("some_date", new BsonString("2002-02-02T02:02:02.002Z")),
    }));
}