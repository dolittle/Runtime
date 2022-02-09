// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_one_conversion;

public class on_an_object : given.a_converter_and_inputs
{
    static BsonValue converted_value;
    
    Establish context = () =>
    {
        state_to_convert = @"
            {
                ""object"":
                {
                    ""some_string"": ""hello world"",
                    ""some_int"": 42
                }
            }
        ";

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "object",
                ConversionBSONType.None,
                false,
                "",
                new[]
                {
                    new PropertyConversion(
                        "some_string",
                        ConversionBSONType.DateAsDate,
                        false,
                        "",
                        Array.Empty<PropertyConversion>()),
                }),
        };

        converted_value = new BsonArray();

        value_converter
            .Setup(_ => _.Convert(new BsonString("hello world"), ConversionBSONType.DateAsDate))
            .Returns(converted_value);
    };

    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_call_the_converter = () => value_converter.Verify(_ => _.Convert(new BsonString("hello world"), ConversionBSONType.DateAsDate), Times.Once);
    It should_have_the_converted_value = () => result["object"]["some_string"].ShouldBeTheSameAs(converted_value);
    It should_leave_the_other_property_alone = () => result["object"]["some_int"].ShouldEqual(new BsonInt32(42));
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
}