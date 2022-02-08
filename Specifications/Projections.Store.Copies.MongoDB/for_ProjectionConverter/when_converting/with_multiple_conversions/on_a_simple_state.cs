// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_multiple_conversions;

public class on_a_simple_state : given.a_converter_and_inputs
{
    static BsonValue converted_string;
    static BsonValue converted_int;
    static BsonValue converted_date;
    
    Establish context = () =>
    {
        state_to_convert = @"
            {
                ""some_string"": ""hello world"",
                ""some_int"": 42,
                ""some_bool"": true,
                ""some_date"": ""2002-02-02T02:02:02.002Z""
            }
        ";
        
        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "some_string",
                ConversionBSONType.DateAsDate,
                false,
                "",
                Array.Empty<PropertyConversion>()),
            new PropertyConversion(
                "some_int",
                ConversionBSONType.DateAsDate,
                false,
                "",
                Array.Empty<PropertyConversion>()),
            new PropertyConversion(
                "some_date",
                ConversionBSONType.DateAsDate,
                false,
                "",
                Array.Empty<PropertyConversion>()),
        };

        converted_string = new BsonArray();
        converted_int = new BsonArray();
        converted_date = new BsonArray();

        value_converter
            .Setup(_ => _.Convert(new BsonString("hello world"), ConversionBSONType.DateAsDate))
            .Returns(converted_string);
        value_converter
            .Setup(_ => _.Convert(new BsonInt32(42), ConversionBSONType.DateAsDate))
            .Returns(converted_int);
        value_converter
            .Setup(_ => _.Convert(new BsonString("2002-02-02T02:02:02.002Z"), ConversionBSONType.DateAsDate))
            .Returns(converted_date);
    };

    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_convert_the_string = () => value_converter.Verify(_ => _.Convert(new BsonString("hello world"), ConversionBSONType.DateAsDate), Times.Once);
    It should_convert_the_int = () => value_converter.Verify(_ => _.Convert(new BsonInt32(42), ConversionBSONType.DateAsDate), Times.Once);
    It should_convert_the_date = () => value_converter.Verify(_ => _.Convert(new BsonString("2002-02-02T02:02:02.002Z"), ConversionBSONType.DateAsDate), Times.Once);
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
    It should_have_the_correct_string = () => result["some_string"].ShouldBeTheSameAs(converted_string);
    It should_have_the_correct_int = () => result["some_int"].ShouldBeTheSameAs(converted_int);
    It should_have_the_correct_bool = () => result["some_bool"].ShouldEqual(new BsonBoolean(true));
    It should_have_the_correct_date = () => result["some_date"].ShouldBeTheSameAs(converted_date);
}