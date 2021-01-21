// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Strings.for_StringExtensions
{
    public class when_converting_a_string_with_camel_casing_to_pascal_casing
    {
        static string result;

        Because of = () => result = "camelCased".ToPascalCase();

        It should_turn_it_into_pascal_case = () => result.ShouldEqual("CamelCased");
    }
}