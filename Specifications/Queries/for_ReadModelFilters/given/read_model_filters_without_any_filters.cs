// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_ReadModelFilters.given
{
    public class read_model_filters_without_any_filters : all_dependencies
    {
        protected static ReadModelFilters filters;

        Establish context = () => filters = new ReadModelFilters(type_discoverer.Object, container.Object);
    }
}
