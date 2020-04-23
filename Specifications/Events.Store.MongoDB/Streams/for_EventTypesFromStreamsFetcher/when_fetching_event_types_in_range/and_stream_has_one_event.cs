// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range
{
    public class and_stream_has_one_event : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static StreamId stream;
        static PartitionId partition;
        static CommittedEvent committed_event;
        static IEnumerable<Artifact> result;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            committed_event = committed_events.a_committed_event(0U);
            var events = an_event_store_connection.GetStreamCollection(stream, CancellationToken.None).GetAwaiter().GetResult();
            events.InsertOne(committed_event.ToStoreStreamEvent(0, partition));
        };

        Because of = () => result = event_types_from_streams.FetchInRange(ScopeId.Default, stream, new StreamPositionRange(0U, 2), CancellationToken.None).GetAwaiter().GetResult();

        It should_not_be_empty_list = () => result.ShouldNotBeEmpty();
        It should_get_one_event_type = () => result.Count().ShouldEqual(1);
        It should_get_the_correct_event_type = () => result.FirstOrDefault().ShouldEqual(committed_event.Type);
    }
}