// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_runtime_stream_event.a_non_aggregate;

public class partitioned_stream_event : given.an_event_content_converter
{
    static MongoDB.Events.StreamEvent stored_event;
    static StreamPosition stream_position;
    static PartitionId partition;
    static StreamId stream;

    static IEventConverter event_converter;
    static Runtime.Events.Store.Streams.StreamEvent result;

    Establish context = () =>
    {
        stream_position = random.stream_position;
        partition = "82e94df4-75d8-45e1-9595-bb17552975e8 partition";
        stream = Guid.Parse("f61ca65f-29e2-47a8-b51a-2ca35e489860");
        stored_event = events.a_stream_event_not_from_aggregate(stream_position, partition);
        event_converter = new EventConverter(event_content_converter.Object);
    };

    Because of = () => result = event_converter.ToRuntimeStreamEvent(stored_event, stream, true);

    It should_return_a_committed_event = () => result.Event.ShouldBeOfExactType<CommittedEvent>();
    It should_have_the_correct_stream_position = () => result.Position.ShouldEqual(stream_position);
    It should_be_partitioned = () => result.Partitioned.ShouldBeTrue();
    It should_come_from_the_correct_partition = () => result.Partition.ShouldEqual(partition);
    It should_come_from_the_correct_stream = () => result.Stream.ShouldEqual(stream);
    It should_have_the_content_returned_by_the_content_converter = () => result.Event.Content.ShouldBeTheSameAs(json_returned_by_event_converter);
    It should_call_the_content_converter_with_the_content = () => event_content_converter.VerifyOnlyCall(_ => _.ToJson(stored_event.Content));
}