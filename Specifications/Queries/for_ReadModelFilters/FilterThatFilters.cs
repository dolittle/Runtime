using System.Collections.Generic;
using doLittle.ReadModels;

namespace doLittle.Queries.Specs.for_ReadModelFilters
{
    public class FilterThatFilters : ICanFilterReadModels
    {
        public bool FilterCalled = false;
        public IEnumerable<IReadModel> Filtered;
        public IEnumerable<IReadModel> Filter(IEnumerable<IReadModel> readModels)
        {
            FilterCalled = true;
            return Filtered;
        }
    }
}
