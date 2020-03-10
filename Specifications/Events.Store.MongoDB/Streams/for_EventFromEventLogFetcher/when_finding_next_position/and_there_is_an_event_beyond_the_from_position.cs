// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventFromEventLogFetcher.when_finding_next_position
{
    public class and_there_is_an_event_beyond_the_from_position : given.all_dependencies
    {
        const uint event_position = 2U;
        static Events.Event stored_event;
        static StreamPosition result;

        Establish context = () =>
        {
            stored_event = events.an_event_not_from_aggregate(event_position);
            an_event_store_connection.EventLog.InsertOne(stored_event);
        };

        Because of = () => result = fetcher.FindNext(StreamId.AllStreamId, PartitionId.NotSet, 0).GetAwaiter().GetResult();
        It should_return_stream_position_of_the_event = () => result.Value.ShouldEqual(event_position);
    }
}