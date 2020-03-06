// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range_and_partition
{
    public class and_stream_has_two_events_with_different_types_in_wrong_partition : given.all_dependencies
    {
        static StreamId stream;
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static IEnumerable<Artifact> result;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            first_committed_event = committed_events.a_committed_event_with_type(0, new Artifact(Guid.NewGuid(), 0));
            second_committed_event = committed_events.a_committed_event_with_type(1, new Artifact(Guid.NewGuid(), 0));
            var events = an_event_store_connection.GetStreamCollectionAsync(stream).GetAwaiter().GetResult();
            events.InsertOne(first_committed_event.ToStoreStreamEvent(0, Guid.NewGuid()));
            events.InsertOne(second_committed_event.ToStoreStreamEvent(1, Guid.NewGuid()));
        };

        Because of = () => result = event_types_from_streams.FetchTypesInRangeAndPartition(stream, Guid.NewGuid(), new StreamPositionRange(0U, 1U)).GetAwaiter().GetResult();

        It should_return_empty_list = () => result.ShouldBeEmpty();
    }
}