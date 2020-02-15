// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsToStreamsWriter
{
    public class when_writing_null_to_stream : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static StreamId stream_id;
        static PartitionId partition;
        static Exception exception;

        Establish context = () =>
        {
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
        };

        Because of = () => exception = Catch.Exception(() => events_to_streams_writer.Write(null, stream_id, partition).GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldNotBeNull();

        It should_fail_because_event_cannot_be_null = () => exception.ShouldBeOfExactType<EventCanNotBeNull>();
    }
}