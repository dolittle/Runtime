// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsToStreamsWriter
{
    public class when_writing_two_events_with_same_event_log_sequence_number_to_stream : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static StreamId stream_id;
        static PartitionId partition;
        static Exception exception;

        Establish context = () =>
        {
            first_committed_event = committed_events.a_committed_event(0);
            second_committed_event = committed_events.a_committed_event(0);
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
            events_to_streams_writer.Write(first_committed_event, stream_id, partition).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => events_to_streams_writer.Write(second_committed_event, stream_id, partition).GetAwaiter().GetResult());

        It should_thrown_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_same_event_is_written_twice = () => exception.ShouldBeOfExactType<EventAlreadyWrittenToStream>();
    }
}