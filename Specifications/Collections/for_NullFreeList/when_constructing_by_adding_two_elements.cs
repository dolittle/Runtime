// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_NullFreeList;

public class when_constructing_by_adding_two_elements
{
    static NullFreeList<string> list;

    Because of = () =>
    {
        list = new NullFreeList<string>();
        list.Add("a");
        list.Add("b");
    };

    It should_contain_two_elements = () => list.Count.ShouldEqual(2);
    It should_have_the_right_first_element = () => list[0].ShouldEqual("a");
    It should_have_the_right_second_element = () => list[1].ShouldEqual("b");
}