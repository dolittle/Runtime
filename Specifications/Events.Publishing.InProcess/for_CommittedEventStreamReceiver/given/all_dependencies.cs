using Dolittle.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Publishing.InProcess.Specs.for_CommittedEventStreamReceiver.given
{
    public class all_dependencies
    {
        protected static Mock<ICommittedEventStreamBridge> committed_event_stream_bridge;
        protected static ILogger logger = Mock.Of<ILogger>();

        Establish context = () => committed_event_stream_bridge = new Mock<ICommittedEventStreamBridge>();
    }
}
