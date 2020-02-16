// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsFromStreamsFetcher.when_finding_next_position.in_a_stream
{
    public class and_first_event_is_in_correct_partition : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static EventsFromStreamsFetcher events_from_streams_fetcher;
        static StreamId stream;
        static PartitionId partition;
        static StreamPosition result;

        Establish context = () =>
        {
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            events_from_streams_fetcher = new EventsFromStreamsFetcher(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            events_to_streams_writer.Write(committed_events.a_committed_event(0U), stream, partition).GetAwaiter().GetResult();
            events_to_streams_writer.Write(committed_events.a_committed_event(1U), stream, Guid.NewGuid()).GetAwaiter().GetResult();
        };

        Because of = () => result = events_from_streams_fetcher.FindNext(stream, partition, 0U).GetAwaiter().GetResult();

        It should_return_stream_position_zero = () => result.Value.ShouldEqual(0U);
    }
}