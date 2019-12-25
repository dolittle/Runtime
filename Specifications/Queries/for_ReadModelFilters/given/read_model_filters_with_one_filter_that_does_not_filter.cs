// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_ReadModelFilters.given
{
    public class read_model_filters_with_one_filter_that_does_not_filter : all_dependencies
    {
        protected static ReadModelFilters filters;
        protected static FilterThatDoesNotFilter filter;

        Establish context = () =>
        {
            type_discoverer.Setup(t => t.FindMultiple<ICanFilterReadModels>()).Returns(new Type[]
            {
                typeof(FilterThatDoesNotFilter)
            });

            filter = new FilterThatDoesNotFilter();
            container.Setup(c => c.Get(typeof(FilterThatDoesNotFilter))).Returns(filter);

            filters = new ReadModelFilters(type_discoverer.Object, container.Object);
        };
    }
}
