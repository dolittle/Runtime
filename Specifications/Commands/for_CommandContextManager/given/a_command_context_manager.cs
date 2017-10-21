using doLittle.Commands;
using doLittle.Events;
using Machine.Specifications;
using Moq;
using doLittle.Execution;
using doLittle.Logging;
using doLittle.Runtime.Events.Coordination;
using doLittle.Runtime.Execution;

namespace doLittle.Runtime.Commands.Specs.for_CommandContextManager.given
{
    public class a_command_context_manager
    {
        protected static CommandContextManager Manager;
        protected static Mock<IUncommittedEventStreamCoordinator> uncommitted_event_stream_coordinator;
        protected static Mock<IExecutionContextManager> execution_context_manager_mock;
        protected static CommandContextFactory factory;
        protected static Mock<ILogger> logger;

        Establish context = () =>
                                        {
                                            CommandContextManager.ResetContext();
                                            uncommitted_event_stream_coordinator = new Mock<IUncommittedEventStreamCoordinator>();
                                            execution_context_manager_mock = new Mock<IExecutionContextManager>();
                                            logger = new Mock<ILogger>();

                                            factory = new CommandContextFactory(
                                                uncommitted_event_stream_coordinator.Object,
                                                execution_context_manager_mock.Object,
                                                logger.Object);
                                           
                                            Manager = new CommandContextManager(factory);
                                        };
    }
}
