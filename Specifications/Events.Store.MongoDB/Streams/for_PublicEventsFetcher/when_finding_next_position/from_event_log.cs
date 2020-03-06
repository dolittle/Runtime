// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsFetcher.when_finding_next_position
{
    public class from_event_log : given.all_dependencies
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() => fetcher.FindNext(StreamId.AllStreamId, PartitionId.NotSet, 0).GetAwaiter().GetResult());

        It should_fail_because_it_cannot_fetch_events_from_the_stream = () => exception.ShouldBeOfExactType<EventsFromWellKnownStreamsFetcherCannotFetchFromStream>();
    }
}