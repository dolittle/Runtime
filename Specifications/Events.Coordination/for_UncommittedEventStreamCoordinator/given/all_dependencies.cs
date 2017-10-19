using doLittle.Runtime.Events.Storage;
using doLittle.Runtime.Events.Publishing;
using doLittle.Runtime.Events.Processing;
using doLittle.Logging;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Events.Coordination.Specs.for_UncommittedEventStreamCoordinator.given
{
    public class all_dependencies
    {
        protected static Mock<IEventStore> event_store;
        protected static Mock<IEventSourceVersions> event_source_versions;
        protected static Mock<ICanSendCommittedEventStream> committed_event_stream_sender;
        protected static Mock<IEventEnvelopes> event_envelopes;
        protected static Mock<IEventSequenceNumbers> event_sequence_numbers;
        protected static Mock<ILogger> logger;

        Establish context = () =>
        {
            event_store = new Mock<IEventStore>();
            event_source_versions = new Mock<IEventSourceVersions>();
            committed_event_stream_sender = new Mock<ICanSendCommittedEventStream>();
            event_envelopes = new Mock<IEventEnvelopes>();
            event_sequence_numbers = new Mock<IEventSequenceNumbers>();
            logger = new Mock<ILogger>();
        };
    }
}
