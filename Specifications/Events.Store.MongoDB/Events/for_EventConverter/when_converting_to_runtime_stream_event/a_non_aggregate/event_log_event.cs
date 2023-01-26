// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;
using FluentAssertions;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_runtime_stream_event.a_non_aggregate;

public class event_log_event : given.an_event_content_converter
{
    static Event stored_event;
    static IEventConverter event_converter;
    static Runtime.Events.Store.Streams.StreamEvent result;

    Establish context = () =>
    {
        stored_event = events.an_event_not_from_aggregate(random.event_log_sequence_number);
        event_converter = new EventConverter(event_content_converter.Object);
    };

    Because of = () => result = event_converter.ToRuntimeStreamEvent(stored_event);

    It should_return_a_committed_event = () => result.Event.Should().BeOfType<CommittedEvent>();
    It should_have_the_correct_committed_event = () => result.Event.ShouldBeTheSameAs(stored_event);
    It should_have_the_correct_stream_position = () => result.Position.Value.Should().Be(stored_event.EventLogSequenceNumber);
    It should_not_be_partitioned = () => result.Partitioned.Should().BeFalse();
    It should_come_from_event_log_stream = () => result.Stream.Should().Be(StreamId.EventLog);
    It should_have_the_content_returned_by_the_content_converter = () => result.Event.Content.Should().Be(json_returned_by_event_converter);
    It should_call_the_content_converter_with_the_content = () => event_content_converter.VerifyOnlyCall(_ => _.ToJson(stored_event.Content));
}