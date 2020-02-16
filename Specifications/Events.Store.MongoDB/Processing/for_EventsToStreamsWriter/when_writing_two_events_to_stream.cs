// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsToStreamsWriter
{
    public class when_writing_two_events_to_stream : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static StreamId stream_id;
        static PartitionId partition;
        static IMongoCollection<Event> stream;

        Establish context = () =>
        {
            first_committed_event = committed_events.a_committed_event(0);
            second_committed_event = committed_events.a_committed_event(1);
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
            stream = an_event_store_connection.GetStreamCollectionAsync(stream_id).GetAwaiter().GetResult();
        };

        Because of = () =>
        {
            Task.WaitAll(
                events_to_streams_writer.Write(first_committed_event, stream_id, partition),
                events_to_streams_writer.Write(second_committed_event, stream_id, partition));
        };

        It should_have_written_two_events_to_stream = () => stream.Find(filters.an_event_filter.Empty).ToList().Count.ShouldEqual(2);
        It should_have_stored_event_at_position_zero = () => stream.Find(filters.an_event_filter.Eq(_ => _.StreamPosition, 0U)).SingleOrDefault().ShouldNotBeNull();
        It should_have_stored_event_at_position_one = () => stream.Find(filters.an_event_filter.Eq(_ => _.StreamPosition, 1U)).SingleOrDefault().ShouldNotBeNull();
        It should_have_stored_the_first_event_with_exactly_the_same_data_as_first_committed_event = () => first_committed_event.ShouldBeStoredWithCorrectStoreRepresentation(stream.Find(filters.an_event_filter.Empty).First(), 0, partition);
        It should_have_stored_the_second_event_with_exactly_the_same_data_as_second_committed_event = () => second_committed_event.ShouldBeStoredWithCorrectStoreRepresentation(stream.Find(filters.an_event_filter.Empty).ToList()[1], 1, partition);
    }
}