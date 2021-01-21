// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Logging;
using Dolittle.Runtime.Resilience;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_ScopedStreamProcessor.given
{
    public class all_dependencies
    {
        protected static EventProcessorId event_processor_id;
        protected static ScopeId scope_id;
        protected static TenantId tenant_id;
        protected static StreamId source_stream_id;
        protected static StreamProcessorId stream_processor_id;
        protected static IResilientStreamProcessorStateRepository stream_processor_state_repository;
        protected static Mock<ICanFetchEventsFromPartitionedStream> events_fetcher;
        protected static IFailingPartitions failing_partitiones;
        protected static Mock<IStreamProcessors> stream_processors;
        protected static Mock<IEventProcessor> event_processor;
        protected static ScopedStreamProcessor stream_processor;
        protected static Mock<IStreamEventWatcher> event_waiter;

        Establish context = () =>
        {
            var events_fetcher_policy = new AsyncPolicyFor<ICanFetchEventsFromStream>(new EventFetcherPolicy(Mock.Of<ILogger<ICanFetchEventsFromStream>>()).Define());
            var in_memory_stream_processor_state_repository = new in_memory_stream_processor_state_repository();
            event_processor_id = Guid.NewGuid();
            scope_id = Guid.NewGuid();
            tenant_id = Guid.NewGuid();
            source_stream_id = Guid.NewGuid();
            stream_processor_id = new StreamProcessorId(scope_id, event_processor_id, source_stream_id);
            stream_processor_state_repository = in_memory_stream_processor_state_repository;
            events_fetcher = new Mock<ICanFetchEventsFromPartitionedStream>();
            event_processor = new Mock<IEventProcessor>();
            event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            event_processor.SetupGet(_ => _.Scope).Returns(scope_id);
            stream_processors = new Mock<IStreamProcessors>();
            failing_partitiones = new FailingPartitions(
                stream_processor_state_repository,
                event_processor.Object,
                events_fetcher.Object,
                events_fetcher_policy,
                Mock.Of<ILogger<FailingPartitions>>());
            event_waiter = new Mock<IStreamEventWatcher>();
            event_waiter.Setup(_ => _.WaitForEvent(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<TimeSpan>(), Moq.It.IsAny<CancellationToken>()));
            stream_processor = new ScopedStreamProcessor(
                tenant_id,
                stream_processor_id,
                new StreamDefinition(new FilterDefinition(source_stream_id, stream_processor_id.EventProcessorId.Value, true)),
                StreamProcessorState.New,
                event_processor.Object,
                stream_processor_state_repository,
                events_fetcher.Object,
                failing_partitiones,
                events_fetcher_policy,
                event_waiter.Object,
                new TimeToRetryForPartitionedStreamProcessor(),
                Mock.Of<ILogger<ScopedStreamProcessor>>());
        };
    }
}