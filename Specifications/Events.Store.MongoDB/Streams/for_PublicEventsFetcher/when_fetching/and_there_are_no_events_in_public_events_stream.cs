// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsFetcher.when_fetching
{
    public class and_there_are_no_events_in_public_events_stream : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => fetcher.Fetch(StreamId.PublicEventsId, 0).GetAwaiter().GetResult());

        It should_fail_because_there_are_no_events_in_the_stream = () => exception.ShouldBeOfExactType<NoEventInStreamAtPosition>();
    }
}