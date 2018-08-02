using Dolittle.Events;
using Dolittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using System.Dynamic;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Coordination;
using Dolittle.Artifacts;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContext.given
{
    public class a_command_context_for_a_simple_command
    {
        protected static CommandRequest command;
        protected static CommandContext command_context;
        protected static Mock<IUncommittedEventStreamCoordinator> uncommitted_event_stream_coordinator;
        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            command = new CommandRequest(TransactionCorrelationId.NotSet, Artifact.New(), new ExpandoObject());
            uncommitted_event_stream_coordinator = new Mock<IUncommittedEventStreamCoordinator>();
            logger = new Mock<ILogger>();
            command_context = new CommandContext(command, null, uncommitted_event_stream_coordinator.Object, logger.Object);
        };
    }
}
