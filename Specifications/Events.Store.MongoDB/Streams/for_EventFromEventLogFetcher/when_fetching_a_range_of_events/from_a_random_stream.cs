// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.when_fetching_a_range_of_events
{
    public class from_a_random_stream : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => fetcher.Fetch(StreamId.New(), 0).GetAwaiter().GetResult());

        It should_fail_because_it_cannot_fetch_events_from_the_stream = () => exception.ShouldBeOfExactType<EventsFromWellKnownStreamsFetcherCannotFetchFromStream>();
    }
}