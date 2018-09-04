namespace Dolittle.Runtime.Events.Specs.Processing
{
    using System;
    using System.Collections.Generic;
    using Dolittle.Artifacts;
    using Dolittle.Logging;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Moq;

    public class given
    {
        public static TestEventProcessor a_test_processor_for(EventProcessorId identifier, Artifact artifact)
        {
            return new TestEventProcessor(identifier,artifact);
        }

        public static TestEventProcessor a_test_processor_for(Artifact artifact)
        {
            return a_test_processor_for(Guid.NewGuid().ToString(),artifact);
        }

        public static Mock<IEventProcessor> an_event_processor_mock()
        {
            Artifact artifact = Artifact.New();
            EventProcessorId id = Guid.NewGuid().ToString();
            var mock = new Mock<IEventProcessor>();
            mock.SetupGet(_=>_.Event).Returns(artifact);
            mock.SetupGet(_=>_.Identifier).Returns(id);
            return mock;
        }
        
        public static Mock<ScopedEventProcessor> a_scoped_event_processor_mock(TenantId tenant, IEventProcessor eventProcessor, ILogger logger = null)
        {
            var offset_repository = new Mock<IEventProcessorOffsetRepository>();
            var unprocessed_event_fetcher = new Mock<IFetchUnprocessedEvents>();

            Func<IEventProcessorOffsetRepository> offset_provider = () => offset_repository.Object;
            Func<IFetchUnprocessedEvents> unprocessed_provider = () => unprocessed_event_fetcher.Object;
            return new Mock<ScopedEventProcessor>(tenant,
                                                    eventProcessor,
                                                    offset_provider,
                                                    unprocessed_provider,
                                                    logger ?? mocks.a_logger().Object);
        }
    }
    public class TestEventProcessor : IEventProcessor
    {
        public List<CommittedEventEnvelope> Processed { get; }

        public TestEventProcessor(EventProcessorId identifier, Artifact @event)
        {
            Identifier = identifier;
            Event = @event;
            Processed = new List<CommittedEventEnvelope>();
        }
        public EventProcessorId Identifier { get; }
        public Artifact Event { get; }

        public void Process(CommittedEventEnvelope eventEnvelope)
        {
            Processed.Add(eventEnvelope);
        }
    }
}