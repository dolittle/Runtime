using System.Collections.Generic;
using doLittle.ReadModels;

namespace doLittle.Queries.Specs.for_ReadModelFilters
{
    public class FilterThatDoesNotFilter : ICanFilterReadModels
    {
        public bool FilterCalled = false;
        public IEnumerable<IReadModel> ReadModelsPassedForFilter = null;
        public IEnumerable<IReadModel> Filter(IEnumerable<IReadModel> readModels)
        {
            FilterCalled = true;
            ReadModelsPassedForFilter = readModels;
            return readModels;
        }
    }
}
