// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.stream_events;

public class and_first_event_is_not_unique : given.all_dependencies
{
    Establish context = () =>
    {
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(0));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(2));
        stored_stream_events.Add(given.a_stream_event.at_event_log_position(3));
        
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(0));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(6));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(7));
        stream_events_to_write.Add(given.a_stream_event.at_event_log_position(10));
    };

    Because of = get_unique_stream_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_all_except_first_event_be_unique = () => unique_stream_events.ShouldContainOnly(stream_events_to_write.Skip(1));
}