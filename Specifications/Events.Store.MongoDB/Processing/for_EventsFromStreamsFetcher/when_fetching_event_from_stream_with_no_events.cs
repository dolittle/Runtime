// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher
{
    public class when_fetching_event_from_stream_with_no_events : given.all_dependencies
    {
        static EventsFromStreamsFetcher events_from_streams_fetcher;
        static StreamId stream;
        static Exception exception;

        Establish context = () =>
        {
            events_from_streams_fetcher = new EventsFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
        };

        Because of = () => exception = Catch.Exception(() => events_from_streams_fetcher.Fetch(stream, 0U).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_stream_has_no_event_at_the_initial_position = () => exception.ShouldBeOfExactType<NoEventInStreamAtPosition>();
    }
}