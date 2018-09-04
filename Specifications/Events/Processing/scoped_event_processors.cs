namespace Dolittle.Runtime.Events.Specs.Processing
{
    using Dolittle.Runtime.Events.Processing;
    using processors = Dolittle.Runtime.Events.Specs.Processing.given;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using System;
    using Moq;

    public class scoped_event_processors 
    {
        protected static ScopedEventProcessor simple_scoped_processor;
        protected static ScopedEventProcessor another_scoped_processor;
        protected static ScopedEventProcessor simple_scoped_processor_for_other_tenant;
        protected static ScopedEventProcessor another_scoped_processor_for_other_tenant;

        protected static TestEventProcessor simple_event_processor;
        protected static TestEventProcessor another_event_processor;
        protected static TestEventProcessor simple_event_processor_for_other_tenant;
        protected static TestEventProcessor another_event_processor_for_other_tenant;

        protected static Mock<IFetchUnprocessedEvents> unprocessed_events_fetcher_for_tenant_simple;
        protected static Mock<IFetchUnprocessedEvents> unprocessed_events_fetcher_for_other_tenant_simple;
        protected static Mock<IFetchUnprocessedEvents> unprocessed_events_fetcher_for_tenant_another;
        protected static Mock<IFetchUnprocessedEvents> unprocessed_events_fetcher_for_other_tenant_another;

        protected static Mock<IEventProcessorOffsetRepository> offset_repository_simple_tenant;
        protected static Mock<IEventProcessorOffsetRepository> offset_repository_another_tenant;
        protected static Mock<IEventProcessorOffsetRepository> offset_repository_simple_other;
        protected static Mock<IEventProcessorOffsetRepository> offset_repository_another_other;

        Establish context = () => 
        {
            unprocessed_events_fetcher_for_tenant_simple = new Mock<IFetchUnprocessedEvents>();
            unprocessed_events_fetcher_for_other_tenant_simple = new Mock<IFetchUnprocessedEvents>();
            unprocessed_events_fetcher_for_tenant_another = new Mock<IFetchUnprocessedEvents>();
            unprocessed_events_fetcher_for_other_tenant_another = new Mock<IFetchUnprocessedEvents>();

            offset_repository_simple_tenant = new Mock<IEventProcessorOffsetRepository>();
            offset_repository_another_tenant = new Mock<IEventProcessorOffsetRepository>();
            offset_repository_simple_other = new Mock<IEventProcessorOffsetRepository>();
            offset_repository_another_other = new Mock<IEventProcessorOffsetRepository>();

            simple_event_processor = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);
            another_event_processor = processors.a_test_processor_for(specs.Artifacts.artifact_for_another_event);

            simple_event_processor_for_other_tenant = processors.a_test_processor_for(specs.Artifacts.artifact_for_simple_event);
            another_event_processor_for_other_tenant = processors.a_test_processor_for(specs.Artifacts.artifact_for_another_event);

            simple_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                simple_event_processor,
                                                                () => offset_repository_simple_tenant.Object,
                                                                () => unprocessed_events_fetcher_for_tenant_simple.Object,
                                                                mocks.a_logger().Object);
            another_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                    another_event_processor,
                                                                    () => offset_repository_another_tenant.Object,
                                                                    () => unprocessed_events_fetcher_for_tenant_another.Object,
                                                                    mocks.a_logger().Object);
            simple_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                                    simple_event_processor_for_other_tenant,
                                                                                    () => offset_repository_simple_other.Object,
                                                                                    () => unprocessed_events_fetcher_for_other_tenant_simple.Object,
                                                                                    mocks.a_logger().Object);
            another_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                                    another_event_processor_for_other_tenant,
                                                                                    () => offset_repository_another_other.Object,
                                                                                    () => unprocessed_events_fetcher_for_other_tenant_another.Object,
                                                                                    mocks.a_logger().Object);
        };
    }
}