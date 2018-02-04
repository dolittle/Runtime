using System.Collections.Generic;
using doLittle.Execution;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerManager.given
{
    public class a_command_handler_manager
    {
        protected static CommandHandlerManager manager;
        protected static Mock<IInstancesOf<ICommandHandlerInvoker>> invokers;

        Establish context = () =>
                            {
                                invokers = new Mock<IInstancesOf<ICommandHandlerInvoker>>();
                                invokers.Setup(_ => _.GetEnumerator()).Returns(new List<ICommandHandlerInvoker>().GetEnumerator());
                                manager = new CommandHandlerManager(invokers.Object);
                            };
    }
}
