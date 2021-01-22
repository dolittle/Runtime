// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_CollectionExtensions
{
    [Subject(typeof(CollectionsExtensions))]
    public class when_combining_no_lookups
    {
        static IEnumerable<ILookup<string, int>> lookups;
        static ILookup<string, int> result;

        Establish context = () => lookups = Enumerable.Empty<ILookup<string, int>>();

        Because of = () => result = lookups.Combine();

        It should_be_empty = () => result.Count().ShouldEqual(0);
    }
}