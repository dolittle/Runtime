// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.without_conversions;

public class a_simple_state : given.a_converter_and_inputs
{
    Establish context = () => state_to_convert = @"
        {
            ""some_string"": ""hello world"",
            ""some_int"": 42,
            ""some_bool"": true,
            ""some_date"": ""2002-02-02T02:02:02.002Z""
        }
    ";

    It should_have_the_correct_string = () => result["some_string"].ShouldEqual(new BsonString("hello world"));
    It should_have_the_correct_int = () => result["some_int"].ShouldEqual(new BsonInt32(42));
    It should_have_the_correct_bool = () => result["some_bool"].ShouldEqual(new BsonBoolean(true));
    It should_have_the_correct_date = () => result["some_date"].ShouldEqual(new BsonString("2002-02-02T02:02:02.002Z"));
}