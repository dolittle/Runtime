// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_writing
{
    public class two_events : given.all_dependencies
    {
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static StreamId stream_id;
        static PartitionId partition;
        static IMongoCollection<MongoDB.Events.StreamEvent> stream;

        Establish context = () =>
        {
            first_committed_event = committed_events.a_committed_event(0);
            second_committed_event = committed_events.a_committed_event(1);
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
            stream = an_event_store_connection.GetStreamCollectionAsync(stream_id).GetAwaiter().GetResult();
        };

        Because of = () =>
        {
            Task.WaitAll(
                public_events_writer.Write(first_committed_event, stream_id, partition),
                public_events_writer.Write(second_committed_event, stream_id, partition));
        };

        It should_have_written_two_events_to_stream = () => stream.Find(filters.a_stream_event_filter.Empty).ToList().Count.ShouldEqual(2);
        It should_have_stored_event_at_position_zero = () => stream.Find(filters.a_stream_event_filter.Eq(_ => _.StreamPosition, 0U)).SingleOrDefault().ShouldNotBeNull();
        It should_have_stored_event_at_position_one = () => stream.Find(filters.a_stream_event_filter.Eq(_ => _.StreamPosition, 1U)).SingleOrDefault().ShouldNotBeNull();
        It should_have_stored_the_first_event_with_exactly_the_same_data_as_first_committed_event = () => first_committed_event.ShouldBeStoredWithCorrectStoreRepresentation(stream.Find(filters.a_stream_event_filter.Empty).First(), 0, partition);
        It should_have_stored_the_second_event_with_exactly_the_same_data_as_second_committed_event = () => second_committed_event.ShouldBeStoredWithCorrectStoreRepresentation(stream.Find(filters.a_stream_event_filter.Empty).ToList()[1], 1, partition);
    }
}