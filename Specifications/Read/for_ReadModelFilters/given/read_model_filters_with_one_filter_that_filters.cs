using System;
using doLittle.Read;
using Machine.Specifications;

namespace doLittle.Specs.Read.for_ReadModelFilters.given
{
    public class read_model_filters_with_one_filter_that_filters : all_dependencies
    {
        protected static ReadModelFilters   filters;
        protected static FilterThatFilters filter;

        Establish context = () =>
        {
            type_discoverer.Setup(t => t.FindMultiple<ICanFilterReadModels>()).Returns(new Type[] {
                typeof(FilterThatFilters)
            });

            filter = new FilterThatFilters();
            container.Setup(c => c.Get(typeof(FilterThatFilters))).Returns(filter);

            filters = new ReadModelFilters(type_discoverer.Object, container.Object);
        };
    }
}
