// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Reflection.Specs.for_TypeExtensions.for_IsEnumerable
{
    public class when_asking_is_an_enumerable_class_enumerable
    {
        static bool IEnumerable_is_enumerable;
        static bool ICollection_is_enumerable;
        static bool array_of_object_is_enumerable;
        static bool IEnumerable_of_string_is_enumerable;
        static bool IEnumerable_of_object_is_enumerable;
        static bool IEnumerable_of_IEnumerable_of_object_is_enumerable;
        static bool IEnumerable_of_complex_type_is_enumerable;
        static bool MyEnumerable_is_enumerable;
        static bool object_is_enumerable;
        static bool string_is_enumerable;
        static bool char_is_enumerable;

        Because of = () =>
        {
            IEnumerable_is_enumerable = typeof(System.Collections.IEnumerable).IsEnumerable();
            ICollection_is_enumerable = typeof(System.Collections.ICollection).IsEnumerable();
            array_of_object_is_enumerable = typeof(object[]).IsEnumerable();
            IEnumerable_of_object_is_enumerable = typeof(IEnumerable<object>).IsEnumerable();
            IEnumerable_of_string_is_enumerable = typeof(IEnumerable<string>).IsEnumerable();
            IEnumerable_of_IEnumerable_of_object_is_enumerable = typeof(IEnumerable<IEnumerable<object>>).IsEnumerable();
            IEnumerable_of_complex_type_is_enumerable = typeof(IEnumerable<ComplexType>).IsEnumerable();
            MyEnumerable_is_enumerable = typeof(MyEnumerable).IsEnumerable();

            object_is_enumerable = typeof(object).IsEnumerable();
            string_is_enumerable = typeof(string).IsEnumerable();
            char_is_enumerable = typeof(char).IsEnumerable();
        };

        It should_consider_IEnumerable_as_enumerable = () => IEnumerable_is_enumerable.ShouldBeTrue();
        It should_consider_ICollection_as_enumerable = () => ICollection_is_enumerable.ShouldBeTrue();
        It should_consider_array_of_object_as_enumerable = () => array_of_object_is_enumerable.ShouldBeTrue();
        It should_consider_IEnumerable_of_string_as_enumerable = () => IEnumerable_of_string_is_enumerable.ShouldBeTrue();
        It should_consider_IEnumerable_of_object_as_enumerable = () => IEnumerable_of_object_is_enumerable.ShouldBeTrue();
        It should_consider_IEnumerable_of_IEnumerable_of_object_is_enumerable = () => IEnumerable_of_IEnumerable_of_object_is_enumerable.ShouldBeTrue();
        It should_consider_IEnumerable_of_complex_type_as_enumerable = () => IEnumerable_of_complex_type_is_enumerable.ShouldBeTrue();
        It should_consider_MyEnumerable_as_enumerable = () => MyEnumerable_is_enumerable.ShouldBeTrue();
        It should_consider_object_as_not_enumerable = () => object_is_enumerable.ShouldBeFalse();
        It should_consider_string_as_not_enumerable = () => string_is_enumerable.ShouldBeFalse();
        It should_consider_char_as_not_enumerable = () => char_is_enumerable.ShouldBeFalse();
    }
}