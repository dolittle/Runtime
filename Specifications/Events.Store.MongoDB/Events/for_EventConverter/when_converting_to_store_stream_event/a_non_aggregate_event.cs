// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_store_stream_event;

public class a_non_aggregate_event : given.an_event_content_converter
{
    static CommittedEvent committed_event;
    static IEventConverter event_converter;
    static StreamPosition stream_position;
    static PartitionId partition;
    static StreamEvent result;

    Establish context = () =>
    {
        committed_event = committed_events.a_committed_event(random.event_log_sequence_number);
        event_converter = new EventConverter(event_content_converter.Object);
        stream_position = random.stream_position;
        partition = "9b63adc5-e09b-4fec-9a30-29c220b0dd79 partition";
    };

    Because of = () => result = event_converter.ToStoreStreamEvent(committed_event, stream_position, partition);

    It should_represent_the_same_event = () => result.ShouldBeTheSameAs(committed_event);
    It should_have_the_correct_stream_position = () => result.StreamPosition.ShouldEqual(stream_position.Value);
    It should_have_the_correct_partition = () => result.Partition.ShouldEqual(partition.Value);
    It should_not_be_applied_by_aggregate = () => result.Aggregate.WasAppliedByAggregate.ShouldBeFalse();
    It should_not_come_from_event_horizon = () => result.EventHorizon.FromEventHorizon.ShouldBeFalse();
    It should_have_the_content_returned_by_the_content_converter = () => result.Content.ShouldBeTheSameAs(bson_returned_by_event_converter);
    It should_call_the_content_converter_with_the_content = () => event_content_converter.VerifyOnlyCall(_ => _.ToBson(committed_event.Content));
}