namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given
{
    using System.Collections.Generic;
    using System.Globalization;
    using Dolittle.Applications;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using given = Dolittle.Runtime.Events.Specs.Processing.given;
    using Machine.Specifications;
    using Moq;

    public class a_test_scoped_event_processing_hub
    {
        protected static TestScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;
        protected static List<CommittedEventStream> commits;

        protected static int execution_context_sets = 0;
        protected static IExecutionContext current_execution_context;

        Establish context = () => 
        {   
            var execution_context = specs.Contexts.get_execution_context();
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(m => m.Current)
                .Returns(execution_context);
            mocked_execution_context_manager.SetupSet(ecm => ecm.Current = Moq.It.IsAny<IExecutionContext>()).Callback<IExecutionContext>(ec => {current_execution_context = ec; execution_context_sets++;});    
            hub = new TestScopedEventProcessingHub(mocks.a_logger().Object,mocked_execution_context_manager.Object);
            commits = given.committed_event_streams();
        }; 
    }
}