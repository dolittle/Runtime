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

    public class a_scoped_event_processor_hub
    {
        protected static ScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;

        Establish context = () => 
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            var execution_context = specs.Contexts.get_execution_context();
            mocked_execution_context_manager.SetupGet(m => m.Current).Returns(execution_context);
            hub = new ScopedEventProcessingHub(mocks.a_logger().Object,mocked_execution_context_manager.Object);
        }; 
    }
}