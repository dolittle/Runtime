// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.stream_events;

public class and_event_log_sequence_numbers_of_all_events_to_write_is_less_than_all_stored_events : given.all_dependencies
{
    Establish context = () =>
    {
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(0));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(2));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(3));
        
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(5));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(6));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(7));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(10));
    };

    Because of = get_unique_stream_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_all_events_be_unique = () => unique_stream_events.ShouldContainOnly(stream_events_to_write);
}