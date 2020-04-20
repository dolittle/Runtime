// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range
{
    public class and_stream_has_two_events_with_the_same_type : given.all_dependencies
    {
        static StreamId stream;
        static PartitionId partition;
        static Artifact event_type;
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static IEnumerable<Artifact> result;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            event_type = new Artifact(Guid.NewGuid(), 0);
            first_committed_event = committed_events.a_committed_event_with_type(0, event_type);
            second_committed_event = committed_events.a_committed_event_with_type(1, event_type);
            var events = an_event_store_connection.GetStreamCollectionAsync(stream).GetAwaiter().GetResult();
            events.InsertOne(first_committed_event.ToStoreStreamEvent(0, partition));
            events.InsertOne(second_committed_event.ToStoreStreamEvent(1, partition));
        };

        Because of = () => result = event_types_from_streams.FetchTypesInRange(stream, new StreamPositionRange(0U, 1U)).GetAwaiter().GetResult();

        It should_not_be_empty_list = () => result.ShouldNotBeEmpty();
        It should_get_two_event_types = () => result.Count().ShouldEqual(2);
        It should_have_the_correct_first_event_type = () => result.ToList()[0].ShouldEqual(event_type);
        It should_have_the_correct_second_event_type = () => result.ToList()[1].ShouldEqual(event_type);
    }
}