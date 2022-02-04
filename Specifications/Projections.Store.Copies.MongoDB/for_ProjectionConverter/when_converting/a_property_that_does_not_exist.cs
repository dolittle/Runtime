// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Projections.Store.Definition.Copies.MongoDB;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Projections.Store.Copies.MongoDB.for_ProjectionConverter.when_converting;

public class a_property_that_does_not_exist : given.a_converter_and_inputs
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

        conversions_to_apply = new[]
        {
            new PropertyConversion(
                "some_other_property",
                ConversionBSONType.Date,
                false,
                "",
                Array.Empty<PropertyConversion>()),
        };
    };

    It should_not_call_the_renamer = () => property_renamer.VerifyNoOtherCalls();
    It should_fail = () => exception.ShouldBeOfExactType<DocumentDoesNotHaveProperty>();
}