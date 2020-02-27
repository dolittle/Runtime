// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;

using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_EventsToStreamsWriter
{
    public class when_writing_an_event_to_the_all_stream : given.all_dependencies
    {
        static readonly StreamId all_stream = StreamId.AllStreamId;
        static readonly PartitionId partition = PartitionId.NotSet;
        static EventsToStreamsWriter events_to_streams_writer;
        static CommittedEvent committed_event;
        static Exception exception;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
        };

        Because of = () => exception = Catch.Exception(() => events_to_streams_writer.Write(committed_event, all_stream, partition).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_it_cannot_write_event_to_all_stream = () => exception.ShouldBeOfExactType<CannotWriteCommittedEventToAllStream>();
    }
}