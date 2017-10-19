using Machine.Specifications;

namespace doLittle.Runtime.Events.Coordination.Specs.for_CommittedEventStreamCoordinator.given
{
    public class a_committed_event_stream_coordinator : all_dependencies
    {
        protected static CommittedEventStreamCoordinator committed_event_stream_coordinator;

        Establish context = () => committed_event_stream_coordinator = new CommittedEventStreamCoordinator(committed_event_stream_receiver_mock.Object, event_processors.Object, event_processor_log.Object, event_processor_states.Object);
    }
}
