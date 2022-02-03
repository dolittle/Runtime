// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_multiple_conversions;

public class on_objects : given.a_converter_and_inputs
{
    static BsonValue converted_int;
    static BsonValue converted_object;
    
    Establish context = () =>
    {
        state_to_convert = @"
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

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "first_object",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "some_int",
                        ConversionBSONType.Date,
                        false,
                        "",
                        Array.Empty<PropertyConversion>()),
                }),
            new PropertyConversion(
                "second_object",
                ConversionBSONType.Date,
                false,
                "",
                Array.Empty<PropertyConversion>()),
        };

        converted_int = new BsonArray();
        converted_object = new BsonArray();

        value_converter
            .Setup(_ => _.Convert(new BsonInt32(42), ConversionBSONType.Date))
            .Returns(converted_int);
        value_converter
            .Setup(_ => _.Convert(Moq.It.IsAny<BsonDocument>(), ConversionBSONType.Date))
            .Returns(converted_object);
    };
    
    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_convert_the_int = () => value_converter.Verify(_ => _.Convert(new BsonInt32(42), ConversionBSONType.Date), Times.Once);
    It should_convert_the_object = () => value_converter.Verify(_ => _.Convert(Moq.It.IsAny<BsonDocument>(), ConversionBSONType.Date), Times.Once);
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
    It should_not_have_touched_the_string = () => result["first_object"]["some_string"].ShouldEqual(new BsonString("hello world"));
    It should_have_the_converted_int = () => result["first_object"]["some_int"].ShouldBeTheSameAs(converted_int);
    It should_have_converted_the_object = () => result["second_object"].ShouldBeTheSameAs(converted_object);
}