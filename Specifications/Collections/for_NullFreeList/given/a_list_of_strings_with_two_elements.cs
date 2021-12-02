// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_NullFreeList.given;

public class a_list_of_strings_with_two_elements
{
    public static NullFreeList<string> list;

    Establish context = () => list = new NullFreeList<string> { "one", "two" };
}