namespace Dolittle.Runtime.Events.Specs.Processing
{
    using System;
    using System.Collections.Generic;
    using Dolittle.Artifacts;
    using Dolittle.Logging;
    using Dolittle.Tenancy;
    using Dolittle.Runtime.Events.Processing;
    using Dolittle.Runtime.Events.Store;
    using Dolittle.Runtime.Tenancy;
    using Dolittle.DependencyInversion;
    using Moq;

    public class given
    {
        public static Mock<IEventProcessor> an_event_processor_mock(Artifact artifact, EventProcessorId id)
        {
            var mock = new Mock<IEventProcessor>();
            mock.SetupGet(_=>_.Event).Returns(artifact);
            mock.SetupGet(_=>_.Identifier).Returns(id);
            return mock;
        }

        public static Mock<IEventProcessor> an_event_processor_mock()
        {
            return an_event_processor_mock(Artifact.New(),Guid.NewGuid());
        }  

        public static Mock<IEventProcessor> an_event_processor_mock(Artifact artifact)
        {
            return an_event_processor_mock(artifact,Guid.NewGuid());
        }       
        public static Mock<IEventProcessorOffsetRepository> an_event_processor_offset_repository_mock()
        {
            var mock = new Mock<IEventProcessorOffsetRepository>();
            mock.Setup(_ => _.Get(Moq.It.IsAny<EventProcessorId>())).Returns(CommittedEventVersion.None);
            return mock;
        }

        public static Mock<IFetchUnprocessedEvents> an_unprocessed_events_fetcher_mock()
        {
            var mock = new Mock<IFetchUnprocessedEvents>();
            mock.Setup(_ => _.GetUnprocessedEvents(Moq.It.IsAny<ArtifactId>(),Moq.It.IsAny<CommittedEventVersion>())).Returns(new SingleEventTypeEventStream(null));
            return mock;
        }

        public static Mock<ScopedEventProcessor> a_scoped_event_processor_mock(TenantId tenant, IEventProcessor eventProcessor, ILogger logger = null)
        {
            var offset_repository = new Mock<IEventProcessorOffsetRepository>();
            var unprocessed_event_fetcher = new Mock<IFetchUnprocessedEvents>();

            FactoryFor<IEventProcessorOffsetRepository> offset_provider = () => offset_repository.Object;
            FactoryFor<IFetchUnprocessedEvents> unprocessed_provider = () => unprocessed_event_fetcher.Object;
            return new Mock<ScopedEventProcessor>(tenant,
                                                    eventProcessor,
                                                    offset_provider,
                                                    unprocessed_provider,
                                                    logger ?? mocks.a_logger().Object);
        }

        public static List<CommittedEventStream> committed_event_streams(CommittedEventVersion startingCommit = null, uint numberOfCommits = 3)
        {
            var commits = new List<CommittedEventStream>();
            CommittedEventVersion version = startingCommit;
            for(int i = 0; i < numberOfCommits;i++)
            {
                var commit = Dolittle.Runtime.Events.Specs.given.Events.Build(version);
                commits.Add(commit);
                version = commit.LastEventVersion;
            }
            return commits;
        }
    }
}