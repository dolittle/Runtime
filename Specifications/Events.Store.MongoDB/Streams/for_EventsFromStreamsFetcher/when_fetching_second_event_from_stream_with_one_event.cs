// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher
{
    public class when_fetching_second_event_from_stream_with_one_event : given.all_dependencies
    {
        static StreamId stream;
        static Exception exception;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            an_event_store_connection.GetStreamCollectionAsync(stream).GetAwaiter().GetResult().InsertOne(events.a_stream_event_not_from_aggregate(0));
        };

        Because of = () => exception = Catch.Exception(() => events_from_streams_fetcher.Fetch(stream, 1U).GetAwaiter().GetResult());

        It should_not_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_not_use_public_events_fetcher = () => public_events_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_stream_has_no_event_at_the_initial_position = () => exception.ShouldBeOfExactType<NoEventInStreamAtPosition>();
    }
}