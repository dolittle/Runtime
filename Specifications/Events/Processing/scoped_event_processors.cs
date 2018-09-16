namespace Dolittle.Runtime.Events.Specs.Processing
{
    using Dolittle.Runtime.Events.Processing;
    using processors = Dolittle.Runtime.Events.Specs.Processing.given;
    using specs = Dolittle.Runtime.Events.Specs.given;
    using Machine.Specifications;
    using System;
    using Moq;
    using System.Collections.Generic;
    using Dolittle.Runtime.Events.Store;
    using System.Linq;
    using Dolittle.Collections;

    public class scoped_event_processors 
    {
        protected static ScopedEventProcessor simple_scoped_processor;
        protected static ScopedEventProcessor another_scoped_processor;
        protected static ScopedEventProcessor simple_scoped_processor_for_other_tenant;
        protected static ScopedEventProcessor another_scoped_processor_for_other_tenant;

        protected static Mock<IEventProcessor> simple_event_processor;
        protected static Mock<IEventProcessor> another_event_processor;
        protected static Mock<IEventProcessor> simple_event_processor_for_other_tenant;
        protected static Mock<IEventProcessor> another_event_processor_for_other_tenant;

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
            unprocessed_events_fetcher_for_tenant_simple = given.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_other_tenant_simple = given.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_tenant_another = given.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_other_tenant_another = given.an_unprocessed_events_fetcher_mock();

            offset_repository_simple_tenant = given.an_event_processor_offset_repository_mock();
            offset_repository_another_tenant = given.an_event_processor_offset_repository_mock();
            offset_repository_simple_other = given.an_event_processor_offset_repository_mock();
            offset_repository_another_other = given.an_event_processor_offset_repository_mock();

            simple_event_processor = processors.an_event_processor_mock(specs.Artifacts.artifact_for_simple_event);
            another_event_processor = processors.an_event_processor_mock(specs.Artifacts.artifact_for_another_event);

            simple_event_processor_for_other_tenant = processors.an_event_processor_mock(specs.Artifacts.artifact_for_simple_event);
            another_event_processor_for_other_tenant = processors.an_event_processor_mock(specs.Artifacts.artifact_for_another_event);

            simple_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                simple_event_processor.Object,
                                                                () => offset_repository_simple_tenant.Object,
                                                                () => unprocessed_events_fetcher_for_tenant_simple.Object,
                                                                mocks.a_logger().Object);
            another_scoped_processor = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                    another_event_processor.Object,
                                                                    () => offset_repository_another_tenant.Object,
                                                                    () => unprocessed_events_fetcher_for_tenant_another.Object,
                                                                    mocks.a_logger().Object);
            simple_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                                    simple_event_processor_for_other_tenant.Object,
                                                                                    () => offset_repository_simple_other.Object,
                                                                                    () => unprocessed_events_fetcher_for_other_tenant_simple.Object,
                                                                                    mocks.a_logger().Object);
            another_scoped_processor_for_other_tenant = new ScopedEventProcessor(specs.Contexts.tenant,
                                                                                    another_event_processor_for_other_tenant.Object,
                                                                                    () => offset_repository_another_other.Object,
                                                                                    () => unprocessed_events_fetcher_for_other_tenant_another.Object,
                                                                                    mocks.a_logger().Object);
        };
    }

    public static class IEventProcessorMockExtensions 
    {
        public static void ShouldHaveProcessedOnly(this Mock<IEventProcessor> mockProcessor, IEnumerable<CommittedEventEnvelope> @events )
        {
            mockProcessor.Verify(p => p.Process(Moq.It.IsAny<CommittedEventEnvelope>()), Times.Exactly(@events.Count()));
            @events.ForEach(e =>
                mockProcessor.Verify(p => p.Process(e), Times.Once())
            );
        }

        public static void ShouldNotHaveProcessedAnyEvents(this Mock<IEventProcessor> mockProcessor)
        {
            mockProcessor.Verify(p => p.Process(Moq.It.IsAny<CommittedEventEnvelope>()), Times.Never());
        }
    }
}