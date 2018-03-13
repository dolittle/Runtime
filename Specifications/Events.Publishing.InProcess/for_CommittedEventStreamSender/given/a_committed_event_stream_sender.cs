using Machine.Specifications;

namespace Dolittle.Runtime.Events.Publishing.InProcess.Specs.for_CommittedEventStreamSender.given
{
    public class a_committed_event_stream_sender : all_dependencies
    {
        protected static CommittedEventStreamSender committed_event_stream_sender;

        Establish context = () => committed_event_stream_sender = new CommittedEventStreamSender(committed_event_stream_bridge_mock.Object, logger);
    }
}
