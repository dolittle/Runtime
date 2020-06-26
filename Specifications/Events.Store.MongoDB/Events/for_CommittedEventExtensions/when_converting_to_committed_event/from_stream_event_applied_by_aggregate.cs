// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions.when_converting_to_committed_event
{
    public class from_stream_event_applied_by_aggregate
    {
        static StreamEvent @event;
        static CommittedEvent result;

        Establish context = () =>
        {
            @event = events.an_aggregate_stream_event(random.stream_position, Guid.Parse("b87abd36-869e-42ac-b969-8a4b38ab4dfe"), random.aggregate_root_version);
        };

        Because of = () => result = @event.ToCommittedEvent();

        It should_return_a_committed_aggregate_event = () => result.ShouldBeOfExactType<CommittedAggregateEvent>();
        It should_represent_the_same_event = () => (result as CommittedAggregateEvent).ShouldBeTheSameAs(@event);
    }
}