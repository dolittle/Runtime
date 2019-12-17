// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.ReadModels;
using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_ReadModelFilters
{
    public class when_filtering_with_one_filter_that_filters : given.read_model_filters_with_one_filter_that_filters
    {
        static ReadModelWithString[] items = new[]
        {
            new ReadModelWithString { Content = "Hello" },
            new ReadModelWithString { Content = "World" },
        };

        static IEnumerable<IReadModel> result = null;
        static ReadModelWithString[] expected = new[] { items[1] };

        Establish context = () => filter.Filtered = expected;

        Because of = () => result = filters.Filter(expected);

        It should_return_the_filtered_result = () => result.ShouldEqual(expected);
        It should_call_the_filter = () => filter.FilterCalled.ShouldBeTrue();
    }
}
