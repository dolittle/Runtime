// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Streams.for_PublicEventsWriter.when_writing
{
    public class two_events_with_same_event_log_sequence_number : given.all_dependencies
    {
        static CommittedEvent first_committed_event;
        static CommittedEvent second_committed_event;
        static PartitionId partition;
        static Exception exception;

        Establish context = () =>
        {
            first_committed_event = committed_events.a_committed_event(0);
            second_committed_event = committed_events.a_committed_event(0);
            partition = Guid.NewGuid();
            public_events_writer.Write(first_committed_event, StreamId.PublicEventsId, partition).GetAwaiter().GetResult();
        };

        Because of = () => exception = Catch.Exception(() => public_events_writer.Write(second_committed_event, StreamId.PublicEventsId, partition).GetAwaiter().GetResult());

        It should_thrown_an_exception = () => exception.ShouldNotBeNull();
        It should_fail_because_same_event_is_written_twice = () => exception.ShouldBeOfExactType<EventAlreadyWrittenToStream>();
    }
}