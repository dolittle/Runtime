using doLittle.DependencyInversion;
using doLittle.Execution;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Specs.Read.Validation.for_QueryValidationDescriptors.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            container = new Mock<IContainer>();
        };
    }
}
