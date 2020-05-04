// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.when_fetching_a_range_of_events
{
    public class and_there_is_one_event_in_event_log : given.all_dependencies
    {
        static MongoDB.Events.Event stored_event;
        static IEnumerable<Runtime.Events.Store.Streams.StreamEvent> result;

        Establish context = () =>
        {
            stored_event = events.an_event_not_from_aggregate(0);
            an_event_store_connection.EventLog.InsertOne(stored_event);
        };

        Because of = () => result = fetcher.FetchRange(ScopeId.Default, StreamId.AllStreamId, new StreamPositionRange(0, 1), CancellationToken.None).GetAwaiter().GetResult();
        It should_fetch_one_event = () => result.Count().ShouldEqual(1);
        It should_return_the_same_event = () => result.First().Event.ShouldBeTheSameAs(stored_event.ToCommittedEvent());
        It should_have_correct_partition = () => result.First().Partition.ShouldEqual(PartitionId.NotSet);
        It should_have_the_correct_stream_id = () => result.First().Stream.ShouldEqual(StreamId.AllStreamId);
    }
}