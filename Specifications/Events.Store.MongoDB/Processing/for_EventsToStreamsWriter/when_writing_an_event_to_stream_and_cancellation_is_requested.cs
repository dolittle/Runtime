// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Dolittle.Logging;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Processing.for_EventsToStreamsWriter
{
    public class when_writing_an_event_to_stream_and_cancellation_is_requested : given.all_dependencies
    {
        static EventsToStreamsWriter events_to_streams_writer;
        static CommittedEvent committed_event;
        static StreamId stream_id;
        static PartitionId partition;
        static CancellationTokenSource cancellation_token_source;
        static Exception exception;

        Establish context = () =>
        {
            committed_event = committed_events.a_committed_event(0);
            events_to_streams_writer = new EventsToStreamsWriter(an_event_store_connection, Moq.Mock.Of<ILogger>());
            stream_id = Guid.NewGuid();
            partition = Guid.NewGuid();
            cancellation_token_source = new CancellationTokenSource(0);
        };

        Because of = () => exception = Catch.Exception(() => events_to_streams_writer.Write(committed_event, stream_id, partition, cancellation_token_source.Token).GetAwaiter().GetResult());

        It should_throw_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_the_task_was_canceled = () => exception.ShouldBeOfExactType<TaskCanceledException>();
    }
}