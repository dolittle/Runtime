namespace Dolittle.Runtime.Events.Specs.Processing.for_ScopedEventProcessorHub.given
{
    using System.Collections.Generic;
    using System.Globalization;
    using Dolittle.Applications;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using processors = Dolittle.Runtime.Events.Specs.Processing.given;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using Moq;

    public class a_scoped_event_processor_hub_configured_with_processors
    {
        protected static ScopedEventProcessor simple_scoped_processor;
        protected static ScopedEventProcessor another_scoped_processor;
        protected static ScopedEventProcessor simple_scoped_processor_for_other_tenant;
        protected static ScopedEventProcessor another_scoped_processor_for_other_tenant;

        protected static TestEventProcessor simple_event_processor;
        protected static TestEventProcessor another_event_processor;
        protected static TestEventProcessor simple_event_processor_for_other_tenant;
        protected static TestEventProcessor another_event_processor_for_other_tenant;
        protected static ScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;

        Establish context = () => 
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(_ => _.Current).Returns(specs.Contexts.get_execution_context());
            hub = new ScopedEventProcessingHub(mocks.a_logger().Object,mocked_execution_context_manager.Object);

            simple_event_processor = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);
            another_event_processor = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);

            simple_event_processor_for_other_tenant = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);
            another_event_processor_for_other_tenant = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);

            simple_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,simple_event_processor,mocks.a_logger().Object);
            another_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,another_event_processor,mocks.a_logger().Object);

            simple_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,simple_event_processor_for_other_tenant,mocks.a_logger().Object);
            another_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,another_event_processor_for_other_tenant,mocks.a_logger().Object);

            hub.Register(simple_scoped_processor);
            hub.Register(another_scoped_processor);
            hub.Register(simple_scoped_processor_for_other_tenant);
            hub.Register(another_scoped_processor_for_other_tenant);
        }; 
    }    
}