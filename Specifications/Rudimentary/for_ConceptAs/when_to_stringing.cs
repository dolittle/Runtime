// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rudimentary.for_ConceptAs
{
    public class when_to_stringing : Rudimentary.given.concepts
    {
        static string result;
        static string result_of_empty_string;
        static string result_of_null_string;

        Because of = () =>
        {
            result = first_string.ToString();
            result_of_empty_string = string_is_empty.ToString();
            result_of_null_string = string_is_null.ToString();
        };

        It should_give_a_string = () => result.ShouldNotBeEmpty();
        It should_give_a_string_from_empty = () => result_of_empty_string.ShouldNotBeEmpty();
        It should_give_a_string_from_null = () => result_of_null_string.ShouldNotBeNull();
    }
}
