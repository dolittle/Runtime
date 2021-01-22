// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_Value
{
    [Subject(typeof(Value<>))]
    public class when_equating_two_instances_with_identical_values_using_the_equals_operator : given.value_objects
    {
        static bool is_equal;

        Because of = () => is_equal = first_value == identical_values_to_first_value;

        It should_be_equal = () => is_equal.ShouldBeTrue();
    }
}