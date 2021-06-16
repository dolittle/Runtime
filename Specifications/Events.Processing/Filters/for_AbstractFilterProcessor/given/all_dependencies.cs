// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;
using Microsoft.Extensions.Logging;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Filters.for_AbstractFilterProcessor.given
{
    public class all_dependencies
    {
        protected static ScopeId scope_id;
        protected static EventProcessorId event_processor_id;
        protected static StreamId stream_id;
        protected static Mock<IWriteEventsToStreams> events_to_streams_writer;
        protected static Mock<AbstractFilterProcessor<IFilterDefinition>> filter_processor;
        protected static CommittedEvent committed_event;

        Establish context = () =>
        {
            committed_event = committed_events.single();

            scope_id = Guid.NewGuid();
            event_processor_id = Guid.NewGuid();
            stream_id = Guid.NewGuid();
            events_to_streams_writer = new Mock<IWriteEventsToStreams>();

            filter_processor = new Mock<AbstractFilterProcessor<IFilterDefinition>>(
                scope_id,
                new FilterDefinition(stream_id.Value, event_processor_id.Value, true),
                events_to_streams_writer.Object,
                Mock.Of<ILogger>());
        };
    }
}
