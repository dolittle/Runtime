// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_PublicEventsFetcher.when_fetching
{
    public class and_there_is_one_event_in_public_events_stream : given.all_dependencies
    {
        static PublicEvent stored_event;
        static Streams.StreamEvent result;

        Establish context = () =>
        {
            stored_event = events.a_public_event(0);
            an_event_store_connection.PublicEvents.InsertOne(stored_event);
        };

        Because of = () => result = fetcher.Fetch(StreamId.PublicEventsId, 0).GetAwaiter().GetResult();
        It should_return_the_same_event = () => result.Event.ShouldBeTheSameAs(stored_event.ToCommittedEvent());
        It should_have_correct_partition = () => result.Partition.ShouldEqual(PartitionId.NotSet);
        It should_have_the_correct_stream_id = () => result.Stream.ShouldEqual(StreamId.PublicEventsId);
    }
}