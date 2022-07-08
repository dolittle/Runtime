// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_UniqueEventsToWriteGetter.when_getting_unique.events;

public class and_there_are_no_events_to_write_or_stored : given.all_dependencies
{

    Because of = get_unique_events;

    It should_be_successful = () => result.ShouldBeTrue();
    It should_have_no_unique_events = () => unique_events.ShouldBeEmpty();
}