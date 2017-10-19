using doLittle.Logging;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Applications.Specs.for_ApplicationResourceResolver.given
{
    public class all_dependencies
    {
        protected static Mock<IApplication> application;
        protected static Mock<IApplicationResourceTypes> application_resource_types;
        protected static Mock<IInstancesOf<ICanResolveApplicationResources>> resolvers;
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IApplicationStructure> application_structure;
        protected static ILogger logger;

        Establish context = () =>
        {
            application_structure = new Mock<IApplicationStructure>();
            application = new Mock<IApplication>();
            application.SetupGet(a => a.Structure).Returns(application_structure.Object);
            application_resource_types = new Mock<IApplicationResourceTypes>();
            resolvers = new Mock<IInstancesOf<ICanResolveApplicationResources>>();
            type_finder = new Mock<ITypeFinder>();
            logger = Mock.Of<ILogger>();
        };
    }
}
