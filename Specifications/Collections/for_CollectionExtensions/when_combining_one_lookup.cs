﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Collections.for_CollectionExtensions;

[Subject(typeof(CollectionsExtensions))]
public class when_combining_one_lookup
{
    static IEnumerable<ILookup<string, int>> lookups;
    static ILookup<string, int> result;

    Establish context = () =>
    {
        lookups = new[]
        {
            new[]
            {
                new { K = "1", V = 1 },
                new { K = "2", V = 2 },
            }.ToLookup(a => a.K, a => a.V)
        };
    };

    Because of = () => result = lookups.Combine();

    It should_have_two_values = () => result.Count.ShouldEqual(2);

    It should_map_first_value = () => result["1"].ShouldContainOnly(1);

    It should_map_second_value = () => result["2"].ShouldContainOnly(2);
}