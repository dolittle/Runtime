using System;
using doLittle.Runtime.Applications;
using doLittle.Commands;
using doLittle.DependencyInversion;
using doLittle.Execution;
using doLittle.Logging;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker.given
{
    public class all_dependencies
    {
        protected static Mock<ITypeFinder> type_finder;
        protected static Mock<IContainer> container;
        protected static Mock<IApplicationResources> application_resources;
        protected static Mock<ICommandRequestConverter> command_request_converter;

        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            type_finder = new Mock<ITypeFinder>();
            type_finder.Setup(t => t.FindMultiple<ICanHandleCommands>()).Returns(new Type[0]);
            container = new Mock<IContainer>();
            application_resources = new Mock<IApplicationResources>();
            command_request_converter = new Mock<ICommandRequestConverter>();
            logger = new Mock<ILogger>();
        };
    }
}
