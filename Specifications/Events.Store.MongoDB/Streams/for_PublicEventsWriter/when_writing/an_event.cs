// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_writing
{
    public class an_event : given.all_dependencies
    {
        static CommittedEvent committed_event;
        static IMongoCollection<PublicEvent> stream;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            stream = an_event_store_connection.PublicEvents;
        };

        Because of = () => public_events_writer.Write(committed_event, StreamId.PublicEventsId, PartitionId.NotSet).GetAwaiter().GetResult();

        It should_have_written_one_event_to_stream = () => stream.Find(filters.public_event_filter.Empty).ToList().Count.ShouldEqual(1);

        It should_have_stored_one_event_at_position_zero = () => stream.Find(filters.public_event_filter.Eq(_ => _.StreamPosition, 0U)).SingleOrDefault().ShouldNotBeNull();

        It should_have_stored_the_event_with_exactly_the_same_data_as_committed_event = () => committed_event.ShouldBeStoredWithCorrectStoreRepresentation(stream.Find(filters.public_event_filter.Empty).First(), 0);
    }
}