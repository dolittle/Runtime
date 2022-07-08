// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.events;

public class and_last_two_events_are_not_unique : given.all_dependencies
{
    Establish context = () =>
    {
        stored_events.Add(given.an_event.at_event_log_position(1));
        stored_events.Add(given.an_event.at_event_log_position(6));
        stored_events.Add(given.an_event.at_event_log_position(10));
        stored_events.Add(given.an_event.at_event_log_position(15));
        
        events_to_write.Add(given.an_event.at_event_log_position(0));
        events_to_write.Add(given.an_event.at_event_log_position(4));
        events_to_write.Add(given.an_event.at_event_log_position(6));
        events_to_write.Add(given.an_event.at_event_log_position(15));
    };

    Because of = get_unique_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_all_except_last_two_events_be_unique = () => unique_events.ShouldContainOnly(events_to_write.SkipLast(2));
}