using Dolittle.Runtime.Events.Publishing;
using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;
using Moq;
using Dolittle.Logging;

namespace Dolittle.Runtime.Events.Coordination.Specs.for_CommittedEventStreamCoordinator.given
{
    public class all_dependencies
    {
        protected static Mock<ICanReceiveCommittedEventStream> committed_event_stream_receiver_mock;
        protected static Mock<IEventProcessors> event_processors;
        protected static Mock<IEventProcessorLog> event_processor_log;
        protected static Mock<IEventProcessorStates> event_processor_states;

        protected static ILogger logger = Mock.Of<ILogger>();

        Establish context = () =>
        {
            committed_event_stream_receiver_mock = new Mock<ICanReceiveCommittedEventStream>();
            event_processors = new Mock<IEventProcessors>();
            event_processor_log = new Mock<IEventProcessorLog>();
            event_processor_states = new Mock<IEventProcessorStates>();
        };
    }
}
