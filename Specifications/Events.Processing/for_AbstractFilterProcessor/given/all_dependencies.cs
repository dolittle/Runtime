// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Logging;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.for_AbstractFilterProcessor.given
{
    public class all_dependencies
    {
        protected static EventProcessorId event_processor_id;
        protected static StreamId stream_id;
        protected static Mock<IWriteEventsToStreams> events_to_streams_writer;
        protected static Mock<AbstractFilterProcessor> filter_processor;
        protected static Store.CommittedEvent committed_event;

        Establish context = () =>
        {
            committed_event = new Store.CommittedEvent(
                EventLogVersion.Initial,
                DateTimeOffset.UtcNow,
                EventSourceId.New(),
                CorrelationId.New(),
                Microservice.New(),
                TenantId.Development,
                new Cause(CauseType.Command, 0),
                new Artifact(ArtifactId.New(), ArtifactGeneration.First),
                "{\"something\":42}");

            event_processor_id = Guid.NewGuid();
            stream_id = Guid.NewGuid();
            events_to_streams_writer = new Mock<IWriteEventsToStreams>();

            filter_processor = new Mock<AbstractFilterProcessor>(
                event_processor_id,
                stream_id,
                events_to_streams_writer.Object,
                Mock.Of<ILogger>());
        };
    }
}