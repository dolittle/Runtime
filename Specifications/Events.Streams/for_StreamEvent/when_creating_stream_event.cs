// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Streams.for_StreamEvent
{
    public class when_creating_stream_event
    {
        static CommittedEvent committed_event;
        static StreamId stream;
        static PartitionId partition;
        static StreamEvent stream_event;

        Establish context = () =>
        {
            stream = Guid.NewGuid();
            partition = Guid.NewGuid();
            committed_event = new CommittedEvent(
                0,
                DateTimeOffset.Now,
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                new Cause(CauseType.Command, 0),
                new Artifacts.Artifact(Guid.NewGuid(), 0),
                false,
                "content");
        };

        Because of = () => stream_event = new StreamEvent(committed_event, stream, partition);

        It should_have_the_correct_stream_id = () => stream_event.Stream.ShouldEqual(stream);
        It should_have_the_correct_partition = () => stream_event.Partition.ShouldEqual(partition);
        It should_have_the_correct_committed_event = () => stream_event.Event.ShouldEqual(committed_event);
    }
}