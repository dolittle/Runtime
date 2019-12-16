// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.DependencyInversion;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store;
using Dolittle.Tenancy;
using Moq;

namespace Dolittle.Runtime.Events.Specs.Processing
{
    public static class given
    {
        public static Mock<IEventProcessor> an_event_processor_mock(Artifact artifact, EventProcessorId id)
        {
            var mock = new Mock<IEventProcessor>();
            mock.SetupGet(_ => _.Event).Returns(artifact);
            mock.SetupGet(_ => _.Identifier).Returns(id);
            return mock;
        }

        public static Mock<IEventProcessor> an_event_processor_mock()
        {
            return an_event_processor_mock(Artifact.New(), Guid.NewGuid());
        }

        public static Mock<IEventProcessor> an_event_processor_mock(Artifact artifact)
        {
            return an_event_processor_mock(artifact, Guid.NewGuid());
        }

        public static Mock<IEventProcessorOffsetRepository> an_event_processor_offset_repository_mock()
        {
#pragma warning disable CA2000
            var inMemory = new InMemoryEventProcessorOffsetRepository();
            var mock = new Mock<IEventProcessorOffsetRepository>();
            mock.Setup(_ => _.Get(Moq.It.IsAny<EventProcessorId>())).Returns<EventProcessorId>((id) => inMemory.Get(id));
            mock.Setup(_ => _.Set(Moq.It.IsAny<EventProcessorId>(), Moq.It.IsAny<CommittedEventVersion>())).Callback<EventProcessorId, CommittedEventVersion>((id, vsn) => inMemory.Set(id, vsn));
            return mock;
        }

        public static Mock<IFetchUnprocessedEvents> an_unprocessed_events_fetcher_mock()
        {
            var mock = new Mock<IFetchUnprocessedEvents>();
            mock.Setup(_ => _.GetUnprocessedEvents(Moq.It.IsAny<ArtifactId>(), Moq.It.IsAny<CommittedEventVersion>())).Returns(new SingleEventTypeEventStream(null));
            return mock;
        }

        public static Mock<ScopedEventProcessor> a_scoped_event_processor_mock(TenantId tenant, IEventProcessor eventProcessor, ILogger logger = null)
        {
            var offset_repository = new Mock<IEventProcessorOffsetRepository>();
            var unprocessed_event_fetcher = new Mock<IFetchUnprocessedEvents>();

            IEventProcessorOffsetRepository offset_provider() => offset_repository.Object;
            FactoryFor<IFetchUnprocessedEvents> unprocessed_provider = () => unprocessed_event_fetcher.Object;
            return new Mock<ScopedEventProcessor>(
                                                tenant,
                                                eventProcessor,
                                                (FactoryFor<IEventProcessorOffsetRepository>)offset_provider,
                                                unprocessed_provider,
                                                logger ?? mocks.a_logger().Object);
        }

        public static List<CommittedEventStream> committed_event_streams(CommittedEventVersion startingCommit = null, uint numberOfCommits = 3)
        {
            var commits = new List<CommittedEventStream>();
            CommittedEventVersion version = startingCommit;
            for (int i = 0; i < numberOfCommits; i++)
            {
                var commit = Dolittle.Runtime.Events.Specs.given.Events.Build(version);
                commits.Add(commit);
                version = commit.LastEventVersion;
            }

            return commits;
        }
    }
}