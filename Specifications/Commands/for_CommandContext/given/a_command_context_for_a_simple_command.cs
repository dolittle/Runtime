using doLittle.Runtime.Applications;
using doLittle.Commands;
using doLittle.Events;
using doLittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using System.Dynamic;
using doLittle.Logging;
using doLittle.Runtime.Events.Coordination;
using doLittle.Runtime.Events.Storage;

namespace doLittle.Runtime.Commands.Specs.for_CommandContext.given
{
    public class a_command_context_for_a_simple_command
    {
        protected static CommandRequest command;
        protected static CommandContext command_context;
        protected static Mock<IUncommittedEventStreamCoordinator> uncommitted_event_stream_coordinator;
        protected static Mock<IEventEnvelopes> event_envelopes;

        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            command = new CommandRequest(TransactionCorrelationId.NotSet, Mock.Of<IApplicationResourceIdentifier>(), new ExpandoObject());
            uncommitted_event_stream_coordinator = new Mock<IUncommittedEventStreamCoordinator>();
            event_envelopes = new Mock<IEventEnvelopes>();
            logger = new Mock<ILogger>();
            command_context = new CommandContext(command, null, uncommitted_event_stream_coordinator.Object, logger.Object);
        };
    }
}
