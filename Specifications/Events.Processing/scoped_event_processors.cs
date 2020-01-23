// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;
using Moq;
using processors = Dolittle.Runtime.Events.Processing.Specs.given;
using specs = Dolittle.Runtime.Events.Specs.given;

namespace Dolittle.Runtime.Events.Processing.Specs
{
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
            unprocessed_events_fetcher_for_tenant_simple = processors.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_other_tenant_simple = processors.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_tenant_another = processors.an_unprocessed_events_fetcher_mock();
            unprocessed_events_fetcher_for_other_tenant_another = processors.an_unprocessed_events_fetcher_mock();

            offset_repository_simple_tenant = processors.an_event_processor_offset_repository_mock();
            offset_repository_another_tenant = processors.an_event_processor_offset_repository_mock();
            offset_repository_simple_other = processors.an_event_processor_offset_repository_mock();
            offset_repository_another_other = processors.an_event_processor_offset_repository_mock();

            simple_event_processor = processors.an_event_processor_mock(specs.Artifacts.artifact_for_simple_event);
            another_event_processor = processors.an_event_processor_mock(specs.Artifacts.artifact_for_another_event);

            simple_event_processor_for_other_tenant = processors.an_event_processor_mock(specs.Artifacts.artifact_for_simple_event);
            another_event_processor_for_other_tenant = processors.an_event_processor_mock(specs.Artifacts.artifact_for_another_event);

            simple_scoped_processor = new ScopedEventProcessor(
                                                                specs.Contexts.tenant,
                                                                simple_event_processor.Object,
                                                                () => offset_repository_simple_tenant.Object,
                                                                () => unprocessed_events_fetcher_for_tenant_simple.Object,
                                                                mocks.a_logger().Object);
            another_scoped_processor = new ScopedEventProcessor(
                                                                specs.Contexts.tenant,
                                                                another_event_processor.Object,
                                                                () => offset_repository_another_tenant.Object,
                                                                () => unprocessed_events_fetcher_for_tenant_another.Object,
                                                                mocks.a_logger().Object);
            simple_scoped_processor_for_other_tenant = new ScopedEventProcessor(
                                                                            specs.Contexts.tenant,
                                                                            simple_event_processor_for_other_tenant.Object,
                                                                            () => offset_repository_simple_other.Object,
                                                                            () => unprocessed_events_fetcher_for_other_tenant_simple.Object,
                                                                            mocks.a_logger().Object);
            another_scoped_processor_for_other_tenant = new ScopedEventProcessor(
                                                                                specs.Contexts.tenant,
                                                                                another_event_processor_for_other_tenant.Object,
                                                                                () => offset_repository_another_other.Object,
                                                                                () => unprocessed_events_fetcher_for_other_tenant_another.Object,
                                                                                mocks.a_logger().Object);
        };
    }
}