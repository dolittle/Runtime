// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.ReadModels;
using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_ReadModelFilters
{
    public class when_filtering_with_one_filter_that_filters_and_one_that_does_not : given.read_model_filters_with_one_filter_that_filters_and_one_that_does_not
    {
        static ReadModelWithString[] items = new[]
        {
            new ReadModelWithString { Content = "Hello" },
            new ReadModelWithString { Content = "World" },
        };

        static IEnumerable<IReadModel> result = null;
        static ReadModelWithString[] expected = new[] { items[1] };

        Establish context = () => filtering_filter.Filtered = expected;

        Because of = () => result = filters.Filter(expected);

        It should_return_the_filtered_result = () => result.ShouldEqual(expected);
        It should_call_the_filtering_filter = () => filtering_filter.FilterCalled.ShouldBeTrue();
        It should_call_the_non_filtering_filter = () => non_filtering_filter.FilterCalled.ShouldBeTrue();
        It should_pass_the_result_from_the_filtering_filter_to_the_non_filtering = () => non_filtering_filter.ReadModelsPassedForFilter.ShouldEqual(expected);
    }
}
