using doLittle.Logging;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Events.Publishing.InProcess.Specs.for_CommittedEventStreamSender.given
{
    public class all_dependencies 
    {
        protected static Mock<ICommittedEventStreamBridge> committed_event_stream_bridge_mock;
        protected static ILogger logger = Mock.Of<ILogger>();

        Establish context = () => committed_event_stream_bridge_mock = new Mock<ICommittedEventStreamBridge>();
    }
}
