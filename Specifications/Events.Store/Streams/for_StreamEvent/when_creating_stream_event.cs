// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Streams.for_StreamEvent;

public class when_creating_stream_event
{
    static CommittedEvent committed_event;
    static StreamId stream;
    static StreamPosition stream_position;
    static EventLogSequenceNumber event_log_sequence;
    static PartitionId partition;
    static StreamEvent stream_event;
    static bool partitioned;
    static Execution.ExecutionContext execution_context;

    Establish context = () =>
    {
        stream = Guid.NewGuid();
        stream_position = 0;
        event_log_sequence = 0;
        partition = "]≈[partitionﬁß";
        execution_context = execution_contexts.create();
        committed_event = new CommittedEvent(
            event_log_sequence,
            DateTimeOffset.Now,
            "event source∞§",
            execution_context,
            new Artifacts.Artifact(Guid.NewGuid(), 0),
            false,
            "content");
        partitioned = false;
    };

    Because of = () => stream_event = new StreamEvent(committed_event, stream_position, stream, partition, partitioned);

    It should_have_the_correct_stream_id = () => stream_event.Stream.ShouldEqual(stream);
    It should_have_the_correct_stream_position = () => stream_event.Position.ShouldEqual(stream_position);
    It should_have_the_correct_partition = () => stream_event.Partition.ShouldEqual(partition);
    It should_have_the_correct_committed_event = () => stream_event.Event.ShouldEqual(committed_event);
    It should_have_the_correct_partitioned_value = () => stream_event.Partitioned.ShouldEqual(partitioned);
}