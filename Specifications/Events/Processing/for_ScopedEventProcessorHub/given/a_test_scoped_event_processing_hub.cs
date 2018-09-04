namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given
{
    using System.Collections.Generic;
    using System.Globalization;
    using Dolittle.Applications;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using Moq;

    public class a_test_scoped_event_processing_hub
    {
        protected static TestScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;
        protected static List<CommittedEventStream> commits;

        Establish context = () => 
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(m => m.Current).Returns(specs.Contexts.get_execution_context());
            hub = new TestScopedEventProcessingHub(mocks.a_logger().Object,mocked_execution_context_manager.Object);
            commits = new List<CommittedEventStream>();
            CommittedEventVersion version = null;
            var first = Dolittle.Runtime.Events.Specs.given.Events.Build(version);
            commits.Add(first);
            var second = Dolittle.Runtime.Events.Specs.given.Events.Build(first.Source.ToCommittedEventVersion(first.Sequence));
            commits.Add(second);
            var third = Dolittle.Runtime.Events.Specs.given.Events.Build(first.Source.ToCommittedEventVersion(first.Sequence));
            commits.Add(third);
        }; 
    }
}