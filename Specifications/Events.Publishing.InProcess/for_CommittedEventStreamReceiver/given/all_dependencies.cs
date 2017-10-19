using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Events.Publishing.InProcess.Specs.for_CommittedEventStreamReceiver.given
{
    public class all_dependencies
    {
        protected static Mock<ICommittedEventStreamBridge> committed_event_stream_bridge;

        Establish context = () => committed_event_stream_bridge = new Mock<ICommittedEventStreamBridge>();
    }
}
