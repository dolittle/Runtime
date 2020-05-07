// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Tenancy;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.for_ScopedStreamProcessor.given
{
    public class all_dependencies
    {
        protected static EventProcessorId event_processor_id;
        protected static ScopeId scope_id;
        protected static TenantId tenant_id;
        protected static StreamId source_stream_id;
        protected static StreamProcessorId stream_processor_id;
        protected static IStreamProcessorStateRepository stream_processor_state_repository;
        protected static Mock<ICanFetchEventsFromStream> events_fetcher;
        protected static Mock<IStreamProcessors> stream_processors;
        protected static Mock<IEventProcessor> event_processor;
        protected static ScopedStreamProcessor stream_processor;

        Establish context = () =>
        {
            var in_memory_stream_processor_state_repository = new in_memory_stream_processor_state_repository();
            event_processor_id = Guid.NewGuid();
            scope_id = Guid.NewGuid();
            tenant_id = Guid.NewGuid();
            source_stream_id = Guid.NewGuid();
            stream_processor_id = new StreamProcessorId(scope_id, event_processor_id, source_stream_id);
            stream_processor_state_repository = in_memory_stream_processor_state_repository;
            events_fetcher = new Mock<ICanFetchEventsFromStream>();
            event_processor = new Mock<IEventProcessor>();
            event_processor.SetupGet(_ => _.Identifier).Returns(event_processor_id);
            event_processor.SetupGet(_ => _.Scope).Returns(scope_id);
            stream_processors = new Mock<IStreamProcessors>();
            stream_processor = new ScopedStreamProcessor(tenant_id, stream_processor_id, StreamProcessorState.New, event_processor.Object, stream_processor_state_repository, events_fetcher.Object, Mock.Of<ILogger<ScopedStreamProcessor>>(), CancellationToken.None);
        };
    }
}