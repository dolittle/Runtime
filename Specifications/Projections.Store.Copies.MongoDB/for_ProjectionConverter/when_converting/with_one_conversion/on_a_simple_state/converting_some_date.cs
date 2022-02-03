// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting.with_one_conversion.on_a_simple_state;

public class converting_some_date : given.a_converter_and_inputs
{
    static BsonValue converted_value;
    
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
        
        conversions_to_apply.Add("some_date", ConversionBSONType.Date);

        converted_value = new BsonArray();
        value_converter
            .Setup(_ => _.Convert(new BsonString("2002-02-02T02:02:02.002Z"), ConversionBSONType.Date))
            .Returns(converted_value);
    };

    static BsonDocument result;

    Because of = () => result = projection_converter.Convert(state_to_convert, conversions_to_apply);

    It should_call_the_converter = () => value_converter.Verify(_ => _.Convert(new BsonString("2002-02-02T02:02:02.002Z"), ConversionBSONType.Date), Times.Once);
    It should_have_the_correct_string = () => result["some_string"].ShouldEqual(new BsonString("hello world"));
    It should_have_the_correct_int = () => result["some_int"].ShouldEqual(new BsonInt32(42));
    It should_have_the_correct_bool = () => result["some_bool"].ShouldEqual(new BsonBoolean(true));
    It should_have_the_converted_value = () => result["some_date"].ShouldBeTheSameAs(converted_value);
    It should_not_convert_anything_else = () => value_converter.VerifyNoOtherCalls();
}