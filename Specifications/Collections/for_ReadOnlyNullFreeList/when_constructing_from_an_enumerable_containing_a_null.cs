// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_ReadOnlyNullFreeList;

public class when_constructing_from_an_enumerable_containing_a_null
{
    static Exception exception;

    Because of = () => exception = Catch.Exception(() =>
    {
        new NullFreeList<string>(new string[] { "a", null, "b" });
    });

    It should_fail = () => exception.ShouldBeOfExactType<ArgumentNullException>();
}