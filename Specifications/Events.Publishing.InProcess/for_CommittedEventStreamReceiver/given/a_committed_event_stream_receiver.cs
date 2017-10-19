using Machine.Specifications;

namespace doLittle.Runtime.Events.Publishing.InProcess.Specs.for_CommittedEventStreamReceiver.given
{
    public class a_committed_event_stream_receiver : all_dependencies
    {
        protected static CommittedEventStreamReceiver committed_event_stream_receiver;

        Establish context = () => committed_event_stream_receiver = new CommittedEventStreamReceiver(committed_event_stream_bridge.Object);
    }
}
