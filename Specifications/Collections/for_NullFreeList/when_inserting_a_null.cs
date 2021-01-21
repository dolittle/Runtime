// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_NullFreeList
{
    public class when_inserting_a_null : given.a_list_of_strings_with_two_elements
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            list.Insert(1, null);
        });

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<ArgumentNullException>();
        It should_still_have_two_elements = () => list.Count.ShouldEqual(2);
        It should_have_the_right_first_element = () => list[0].ShouldEqual("one");
        It should_have_the_right_second_element = () => list[1].ShouldEqual("two");
    }
}