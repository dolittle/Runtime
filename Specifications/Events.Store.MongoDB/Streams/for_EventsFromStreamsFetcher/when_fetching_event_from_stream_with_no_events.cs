// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher
{
    public class when_fetching_event_from_stream_with_no_events : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => events_from_streams_fetcher.Fetch(ScopeId.Default, Guid.NewGuid(), 0U, CancellationToken.None).GetAwaiter().GetResult());

        It should_not_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<ScopeId>(), Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<CancellationToken>()), Moq.Times.Never);
        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_stream_has_no_event_at_the_initial_position = () => exception.ShouldBeOfExactType<NoEventInStreamAtPosition>();
    }
}