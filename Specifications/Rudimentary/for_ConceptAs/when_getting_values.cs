// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rudimentary.for_ConceptAs;

public class when_getting_values : Rudimentary.given.concepts
{
    static string result_of_empty_string;
    static string result_of_null_string;
    static string result_of_null_value_string;

    Because of = () =>
    {
        result_of_empty_string = string_is_empty.Value;
        result_of_null_string = (string)string_is_null;
        result_of_null_value_string = (string)string_value_is_null;
    };

    It should_be_an_empty_string = () => result_of_empty_string.ShouldBeTheSameAs("");
    It should_be_a_default_string = () => result_of_null_string.ShouldBeTheSameAs(default(string));
    It should_be_a_default_string_from_value = () => result_of_null_value_string.ShouldBeTheSameAs(default(string));
}