// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Rudimentary.for_ConceptAs
{
    public class when_getting_values : Rudimentary.given.concepts
    {
        static string result_of_empty_string;
        static string result_of_null_string;

        Because of = () =>
        {
            result_of_empty_string = string_is_empty.Value;
            result_of_null_string = string_is_null.Value;
        };

        It should_be_an_empty_string = () => result_of_empty_string.ShouldBeTheSameAs("");
        It should_be_a_default_string = () => result_of_null_string.ShouldBeTheSameAs(default(string));
    }
}
