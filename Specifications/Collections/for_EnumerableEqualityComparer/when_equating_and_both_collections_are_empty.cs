// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_EnumerableEqualityComparer
{
    [Subject(typeof(EnumerableEqualityComparer<>))]
    public class when_equating_and_both_collections_are_empty
    {
        static IEqualityComparer<IEnumerable<int>> comparer;
        static bool is_equal;

        Establish context = () => comparer = new EnumerableEqualityComparer<int>();

        Because of = () => is_equal = comparer.Equals(Enumerable.Empty<int>(), Enumerable.Empty<int>());

        It should_be_equal = () => is_equal.ShouldBeTrue();
    }
}