// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher
{
    public class when_fetching_first_event_from_stream_with_one_event : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static StreamId stream;
        static PartitionId partition;
        static Events.StreamEvent stored_event;
        static Streams.StreamEvent result;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            stored_event = events.new_stream_event_not_from_aggregate(0)
                .with_event_log_sequence_number(0)
                .with_partition(partition).build();

            an_event_store_connection.GetStreamCollectionAsync(stream).GetAwaiter().GetResult().InsertOne(stored_event);
        };

        Because of = () => result = events_from_streams_fetcher.Fetch(stream, 0U).GetAwaiter().GetResult();

        It should_not_use_events_from_event_log_fetcher = () => events_from_event_log_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_not_use_public_events_fetcher = () => public_events_fetcher.Verify(_ => _.Fetch(Moq.It.IsAny<StreamId>(), Moq.It.IsAny<StreamPosition>(), Moq.It.IsAny<System.Threading.CancellationToken>()), Moq.Times.Never);
        It should_fetch_an_event = () => result.ShouldNotBeNull();
        It should_fetch_event_with_the_correct_partition_id = () => result.Partition.ShouldEqual(partition);
        It should_fetch_event_with_the_stream_id = () => result.Stream.ShouldEqual(stream);
        It should_fetch_the_correct_event = () => result.Event.ShouldBeTheSameAs(stored_event.ToCommittedEvent());
    }
}