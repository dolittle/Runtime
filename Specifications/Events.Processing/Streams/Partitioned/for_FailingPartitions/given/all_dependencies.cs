// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Dolittle.Runtime.Execution;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Events.Processing.Streams.Partitioned.for_FailingPartitions.given;

public class all_dependencies
{
    protected static StreamId stream_id;
    // protected static Mock<IStreamProcessorStates> stream_processor_state_repository;
    protected static IStreamProcessorStates stream_processor_state_repository;
    protected static Mock<IEventProcessor> event_processor;
    protected static Mock<ICanFetchEventsFromPartitionedStream> events_fetcher;
    protected static Func<StreamEvent, ExecutionContext> create_execution_context;

    Establish context = () =>
    {
        stream_id = Guid.NewGuid();
        event_processor = new Mock<IEventProcessor>();
        stream_processor_state_repository = new in_memory_stream_processor_states();
        events_fetcher = new Mock<ICanFetchEventsFromPartitionedStream>();
        create_execution_context = stream_event => stream_event.Event.ExecutionContext;
    };
}