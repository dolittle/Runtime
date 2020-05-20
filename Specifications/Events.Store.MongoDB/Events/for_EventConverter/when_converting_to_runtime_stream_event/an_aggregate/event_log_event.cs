// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_runtime_stream_event.an_aggregate
{
    public class event_log_event
    {
        static Event stored_event;
        static IEventConverter event_converter;
        static Runtime.Events.Store.Streams.StreamEvent result;

        Establish context = () =>
        {
            stored_event = events.an_aggregate_event(random.event_log_sequence_number, random.aggregate_root_version);
            event_converter = new EventConverter();
        };

        Because of = () => result = event_converter.ToRuntimeStreamEvent(stored_event);

        It should_return_a_committed_aggregate_event = () => result.Event.ShouldBeOfExactType<CommittedAggregateEvent>();
        It should_have_the_correct_committed_event = () => (result.Event as CommittedAggregateEvent).ShouldBeTheSameAs(stored_event);
        It should_have_the_correct_stream_position = () => result.Position.Value.ShouldEqual(stored_event.EventLogSequenceNumber);
        It should_not_be_partitioned = () => result.Partitioned.ShouldBeFalse();
        It should_come_from_event_log_stream = () => result.Stream.ShouldEqual(StreamId.EventLog);
    }
}
