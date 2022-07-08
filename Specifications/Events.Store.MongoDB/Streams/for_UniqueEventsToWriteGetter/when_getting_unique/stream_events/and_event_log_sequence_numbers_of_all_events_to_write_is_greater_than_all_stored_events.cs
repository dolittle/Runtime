// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.stream_events;

public class and_event_log_sequence_numbers_of_all_events_to_write_is_greater_than_all_stored_events : given.all_dependencies
{
    Establish context = () =>
    {
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(0));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(2));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(3));
        
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(5));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(6));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(7));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(10));
    };

    Because of = get_unique_stream_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_all_events_be_unique = () => unique_stream_events.ShouldContainOnly(stream_events_to_write);
}


public class and_there_are_different_events_with_the_same_sequence_number : given.all_dependencies
{
    Establish context = () =>
    {
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(0));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(2).with_event_source("source"));

        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(1));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(2).with_event_source("some source")); ;
    };

    Because of = get_unique_stream_events;

    It should_not_be_successful = () => result.ShouldBeFalse();
    It should_have_the_correct_duplicate_event_log_sequence_number = () => duplicate_event_log_sequence_number.Value.ShouldEqual(2ul);
}

