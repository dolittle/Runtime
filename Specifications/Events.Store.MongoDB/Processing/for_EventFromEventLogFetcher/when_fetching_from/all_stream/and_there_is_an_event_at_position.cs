// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventFromEventLogFetcher.when_fetching_from.all_stream
{
    public class and_there_is_an_event_at_position : given.all_dependencies
    {
        static Events.Event stored_event;
        static Streams.StreamEvent result;

        Establish context = () =>
        {
            stored_event = events.an_event_not_from_aggregate(0);
            an_event_store_connection.EventLog.InsertOne(stored_event);
        };

        Because of = () => result = fetcher.Fetch(StreamId.AllStreamId, 0).GetAwaiter().GetResult();
        It should_return_the_same_event = () => result.Event.ShouldBeTheSameAs(stored_event.ToCommittedEvent());
        It should_have_correct_partition = () => result.Partition.ShouldEqual(PartitionId.NotSet);
        It should_have_the_correct_stream_id = () => result.Stream.ShouldEqual(StreamId.AllStreamId);
    }
}