// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher
{
    public class when_fetching_event_from_stream_with_no_events : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => events_from_streams_fetcher.Fetch(Guid.NewGuid(), 0U).GetAwaiter().GetResult());

        It should_not_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_not_use_public_events_fetcher = () => public_events_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_stream_has_no_event_at_the_initial_position = () => exception.ShouldBeOfExactType<NoEventInStreamAtPosition>();
    }
}