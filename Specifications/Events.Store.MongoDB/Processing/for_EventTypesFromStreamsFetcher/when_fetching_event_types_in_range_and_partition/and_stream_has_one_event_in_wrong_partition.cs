// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Dolittle.Artifacts;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventTypesFromStreamsFetcher.when_fetching_event_types_in_range_and_partition
{
    public class and_stream_has_one_event_in_wrong_partition : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static EventTypesFromStreamsFetcher event_types_from_streams;
        static StreamId stream;
        static CommittedEvent committed_event;
        static IEnumerable<Artifact> result;

        Establish context = () =>
        {
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            event_types_from_streams = new EventTypesFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
            committed_event = committed_events.a_committed_event(0U);
            events_to_streams_writer.Write(committed_event, stream, Guid.NewGuid()).GetAwaiter().GetResult();
        };

        Because of = () => result = event_types_from_streams.FetchTypesInRangeAndPartition(stream, Guid.NewGuid(), 0U, 0U).GetAwaiter().GetResult();

        It should_return_empty_list = () => result.ShouldBeEmpty();
    }
}