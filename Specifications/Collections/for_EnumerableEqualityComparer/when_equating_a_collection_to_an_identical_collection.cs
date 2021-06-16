// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_EnumerableEqualityComparer
{
    [Subject(typeof(EnumerableEqualityComparer<>))]
    public class when_equating_a_collection_to_an_identical_collection
    {
        static IEnumerable<int> collection;
        static IEnumerable<int> copy;
        static IEqualityComparer<IEnumerable<int>> comparer;
        static bool is_equal;

        Establish context = () =>
        {
            collection = new int[] { 1, 2, 3 };
            copy = collection.ToArray();
            comparer = new EnumerableEqualityComparer<int>();
        };

        Because of = () => is_equal = comparer.Equals(collection, copy);

        It should_be_equal = () => is_equal.ShouldBeTrue();
    }
}