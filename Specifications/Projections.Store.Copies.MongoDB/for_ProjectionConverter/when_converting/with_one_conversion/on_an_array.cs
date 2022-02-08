// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_one_conversion;

public class on_an_array : given.a_converter_and_inputs
{
    static BsonValue converted_a;
    static BsonValue converted_b;
    static BsonValue converted_c;
    
    Establish context = () =>
    {
        state_to_convert = @"
            {
                ""an_array_of_strings"": [""a"", ""b"", ""c""],
            }
        ";

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "an_array_of_strings",
                ConversionBSONType.DateAsDate,
                false,
                "",
                Array.Empty<PropertyConversion>()),
        };

        converted_a = new BsonArray();
        converted_b = new BsonArray();
        converted_c = new BsonArray();

        value_converter
            .Setup(_ => _.Convert(new BsonString("a"), ConversionBSONType.DateAsDate))
            .Returns(converted_a);
        value_converter
            .Setup(_ => _.Convert(new BsonString("b"), ConversionBSONType.DateAsDate))
            .Returns(converted_b);
        value_converter
            .Setup(_ => _.Convert(new BsonString("c"), ConversionBSONType.DateAsDate))
            .Returns(converted_c);
    };

    It should_call_the_renamer = () => property_renamer.Verify(_ => _.RenamePropertiesIn(Moq.It.IsAny<BsonDocument>(), conversions_to_apply), Times.Once);
    It should_call_the_value_converter_with_a = () => value_converter.Verify(_ => _.Convert(new BsonString("a"), ConversionBSONType.DateAsDate), Times.Once);
    It should_call_the_value_converter_with_b = () => value_converter.Verify(_ => _.Convert(new BsonString("b"), ConversionBSONType.DateAsDate), Times.Once);
    It should_call_the_value_converter_with_c = () => value_converter.Verify(_ => _.Convert(new BsonString("c"), ConversionBSONType.DateAsDate), Times.Once);
    It should_have_the_converted_values = () => result["an_array_of_strings"].AsBsonArray.ShouldContainOnly(converted_a, converted_b, converted_c);
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
}