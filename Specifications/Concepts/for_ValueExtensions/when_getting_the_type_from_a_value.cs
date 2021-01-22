// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Concepts;
using Machine.Specifications;

namespace Dolittle.Runtime.Specs.Concepts.for_ValueExtensions
{
    [Subject(typeof(ValueExtensions), nameof(ValueExtensions.IsValue))]
    public class when_getting_the_type_from_a_value
    {
        static Type test_value_type;
        static Type test_value_type_from_instance;
        static Type obj_type;
        static Type obj_type_from_instance;

        Because of = () =>
        {
            test_value_type = typeof(TestValue).GetValueType();
            test_value_type_from_instance = new TestValue().GetValueType();
            obj_type = typeof(object).GetValueType();
            obj_type_from_instance = new object().GetValueType();
        };

        It should_be_the_type_for_the_value_type = () => test_value_type.ShouldEqual(typeof(TestValue));
        It should_be_the_type_for_the_value_instance = () => test_value_type_from_instance.ShouldEqual(typeof(TestValue));
        It should_be_null_for_a_non_value_type = () => obj_type.ShouldBeNull();
        It should_be_null_for_a_non_value_instance = () => obj_type_from_instance.ShouldBeNull();
    }
}