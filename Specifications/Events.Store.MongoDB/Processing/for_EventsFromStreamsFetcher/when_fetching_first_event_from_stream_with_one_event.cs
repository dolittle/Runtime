// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher
{
    public class when_fetching_first_event_from_stream_with_one_event : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static EventsFromStreamsFetcher events_from_streams_fetcher;
        static StreamId stream;
        static PartitionId partition;
        static CommittedEvent committed_event;
        static StreamEvent result;

        Establish context = () =>
        {
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            events_from_streams_fetcher = new EventsFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            events_to_streams_writer.Write(committed_events.a_committed_event(0U), stream, partition).GetAwaiter().GetResult();
        };

        Because of = () => result = events_from_streams_fetcher.Fetch(stream, 0U).GetAwaiter().GetResult();

        It should_fetch_an_event = () => result.ShouldNotBeNull();

        It should_fetch_event_with_the_correct_partition_id = () => result.Partition.ShouldEqual(partition);

        It should_fetch_the_correct_event = () => result.Event.ShouldBeTheSameAs(result.Event);
    }
}