using doLittle.DependencyInversion;
using doLittle.Execution;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Specs.Read.for_ReadModelFilters.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_discoverer;
        protected static Mock<IContainer> container;

        Establish context = () =>
        {
            type_discoverer = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
        };
    }
}
