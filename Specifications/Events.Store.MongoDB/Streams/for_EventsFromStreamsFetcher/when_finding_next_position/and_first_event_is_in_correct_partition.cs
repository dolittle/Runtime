// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsFromStreamsFetcher.when_finding_next_position.in_a_stream
{
    public class and_first_event_is_in_correct_partition : given.all_dependencies
    {
        static StreamId stream;
        static PartitionId partition;
        static StreamPosition result;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            var events = an_event_store_connection.GetStreamCollection(stream, CancellationToken.None).GetAwaiter().GetResult();
            events.InsertOne(committed_events.a_committed_event(0U).ToStoreStreamEvent(0, partition));
            events.InsertOne(committed_events.a_committed_event(1).ToStoreStreamEvent(1, Guid.NewGuid()));
        };

        Because of = () => result = events_from_streams_fetcher.FindNext(ScopeId.Default, stream, partition, 0U, CancellationToken.None).GetAwaiter().GetResult();

        It should_return_stream_position_zero = () => result.Value.ShouldEqual(0U);
    }
}