using Machine.Specifications;

namespace doLittle.Runtime.Events.Coordination.Specs.for_UncommittedEventStreamCoordinator.given
{
    public class an_uncommitted_event_stream_coordinator : all_dependencies
    {
        protected static UncommittedEventStreamCoordinator coordinator;

        Establish context = () => coordinator = 
            new UncommittedEventStreamCoordinator(
                event_store.Object, 
                event_source_versions.Object,
                committed_event_stream_sender.Object, 
                event_envelopes.Object,
                event_sequence_numbers.Object,
                logger.Object);
    }
}
