// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using MongoDB.Bson;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting;

public class a_property_on_a_number : given.a_converter_and_inputs
{
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
        
        conversions_to_apply.Add("some_int.some_property", ConversionBSONType.Date);
    };

    static Exception exception;

    Because of = () => exception = Catch.Exception(() => projection_converter.Convert(state_to_convert, conversions_to_apply));

    It should_fail = () => exception.ShouldBeOfExactType<ValueIsNotDocument>();
}