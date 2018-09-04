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

    public class a_scoped_event_processor_hub_configured_with_processors : scoped_event_processors
    {

        protected static ScopedEventProcessingHub hub;
        protected static Mock<IExecutionContextManager> mocked_execution_context_manager;

        Establish context = () => 
        {
            mocked_execution_context_manager = mocks.an_execution_context_manager();
            mocked_execution_context_manager.SetupGet(_ => _.Current).Returns(specs.Contexts.get_execution_context());
            hub = new ScopedEventProcessingHub(mocks.a_logger().Object,mocked_execution_context_manager.Object);

            hub.Register(simple_scoped_processor);
            hub.Register(another_scoped_processor);
            hub.Register(simple_scoped_processor_for_other_tenant);
            hub.Register(another_scoped_processor_for_other_tenant);
        }; 
    }    
}