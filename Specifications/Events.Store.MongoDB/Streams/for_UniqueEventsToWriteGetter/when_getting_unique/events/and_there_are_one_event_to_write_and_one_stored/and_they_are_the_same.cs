// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.events.and_there_are_one_event_to_write_and_one_stored;

public class and_they_are_the_same : given.all_dependencies
{
    Establish context = () =>
    {
        events_to_write.Add(given.an_event.at_event_log_position(0));
        stored_events.Add(given.an_event.at_event_log_position(0));
    };

    Because of = get_unique_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_none_unique_events = () => unique_events.ShouldBeEmpty();
}