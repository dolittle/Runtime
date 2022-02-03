// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.without_conversions;

public class arrays : given.a_converter_and_inputs
{
    Establish context = () => state_to_convert = @"
        {
            ""an_array_of_strings"": [""a"", ""b"", ""c""],
            ""an_array_of_ints"": [1, 2, 3],
            ""an_array_of_bool"": [true, false, false],
            ""an_array_of_dates"": [""2001-01-01T01:01:01.001Z"",""2002-02-02T02:02:02.002Z""]
        }
    ";
    
    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_have_the_correct_strings = () => result["an_array_of_strings"].AsBsonArray.ShouldContainOnly(
        new BsonString("a"), new BsonString("b"), new BsonString("c"));
    It should_have_the_correct_ints = () => result["an_array_of_ints"].AsBsonArray.ShouldContainOnly(
        new BsonInt32(1), new BsonInt32(2), new BsonInt32(3));
    It should_have_the_correct_bools = () => result["an_array_of_bool"].AsBsonArray.ShouldContainOnly(
        new BsonBoolean(true), new BsonBoolean(false), new BsonBoolean(false));
    It should_have_the_correct_dates = () => result["an_array_of_dates"].AsBsonArray.ShouldContainOnly(
        new BsonString("2001-01-01T01:01:01.001Z"), new BsonString("2002-02-02T02:02:02.002Z"));
}