// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher.when_finding_next_position.in_the_all_stream
{
    public class and_first_event_is_in_requested_partition : given.all_dependencies
    {
        static EventsFromStreamsFetcher events_from_streams_fetcher;
        static PartitionId partition;
        static StreamPosition result;

        Establish context = () =>
        {
            events_from_streams_fetcher = new EventsFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            partition = Guid.NewGuid();
            an_event_store_connection.AllStream.InsertOne(events.an_event_not_from_aggregate_with_partition(0, 0, partition));
        };

        Because of = () => result = events_from_streams_fetcher.FindNext(StreamId.AllStreamId, partition, 0U).GetAwaiter().GetResult();

        It should_return_stream_position_zero = () => result.Value.ShouldEqual(0U);
    }
}