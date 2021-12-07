// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_EnumerableEqualityComparer;

[Subject(typeof(EnumerableEqualityComparer<>))]
public class when_equating_a_collection_to_a_collection_which_is_a_superset
{
    static IEnumerable<int> collection;
    static IEnumerable<int> other;
    static IEqualityComparer<IEnumerable<int>> comparer;
    static bool is_equal;

    Establish context = () =>
    {
        collection = new int[] { 1, 2, 3 };
        other = new int[] { 1, 2, 3, 4 };
        comparer = new EnumerableEqualityComparer<int>();
    };

    Because of = () => is_equal = comparer.Equals(collection, other);

    It should_not_be_equal = () => is_equal.ShouldBeFalse();
}