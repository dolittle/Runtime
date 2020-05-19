// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Events.Store.Streams;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_EventConverter.when_converting_to_runtime_stream_event.an_external
{
    public class unpartitioned_stream_event
    {
        static MongoDB.Events.StreamEvent stored_event;
        static StreamPosition stream_position;
        static StreamId stream;

        static IEventConverter event_converter;
        static Runtime.Events.Store.Streams.StreamEvent result;

        Establish context = () =>
        {
            stream_position = 2;
            stream = Guid.NewGuid();
            stored_event = events.an_external_stream_event(stream_position, Guid.Empty);
            event_converter = new EventConverter();
        };

        Because of = () => result = event_converter.ToRuntimeStreamEvent(stored_event, stream, false);

        It should_return_a_committed_external_event = () => result.Event.ShouldBeOfExactType<CommittedExternalEvent>();
        It should_have_the_correct_committed_event = () => (result.Event as CommittedExternalEvent).ShouldBeTheSameAs(stored_event);
        It should_have_the_correct_stream_position = () => result.Position.ShouldEqual(stream_position);
        It should_not_be_partitioned = () => result.Partitioned.ShouldBeFalse();
        It should_come_from_the_correct_stream = () => result.Stream.ShouldEqual(stream);
    }
}
