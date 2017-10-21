using doLittle.Read;
using Machine.Specifications;

namespace doLittle.Specs.Read.for_ReadModelFilters.given
{
    public class read_model_filters_without_any_filters : all_dependencies
    {
        protected static ReadModelFilters filters;

        Establish context = () =>
        {
            filters = new ReadModelFilters(type_discoverer.Object, container.Object);
        };
    }
}
