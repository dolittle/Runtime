// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Processing;
using Dolittle.Runtime.Events.Store.MongoDB.Events;
using Machine.Specifications;
using MongoDB.Driver;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsToStreamsWriter
{
    public class when_writing_null_to_stream : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
        static StreamId stream_id;
        static PartitionId partition;
        static IMongoCollection<Event> stream;
        static Exception exception;

        Establish context = () =>
        {
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
            stream = an_event_store_connection.GetStreamCollectionAsync(stream_id).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => events_to_streams_writer.Write(null, stream_id, partition).GetAwaiter().GetResult());

        It should_throw_an_exception = () => exception.ShouldNotBeNull();
    }
}