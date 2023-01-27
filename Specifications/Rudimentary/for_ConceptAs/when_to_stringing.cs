// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Rudimentary.for_ConceptAs;

public class when_to_stringing : Rudimentary.given.concepts
{
    static string result;
    static string result_of_empty_string;
    static string result_of_null_string;

    Because of = () =>
    {
        result = first_string.ToString();
        result_of_empty_string = string_is_empty.ToString();
        result_of_null_string = string_value_is_null.ToString();
    };

    It should_return_the_first_string_itself = () => result.Should().Be(first_string.Value);
    It should_return_the_empty_string_for_empty_string_concept = () => result_of_empty_string.Should().Be(string.Empty);
    It should_return_NULL_string_from_null = () => result_of_null_string.Should().Be("NULL");
}