// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events.for_CommittedEventExtensions.when_converting_to_committed_event
{
    public class from_event_applied_by_aggregate
    {
        static Event @event;
        static CommittedEvent result;

        Establish context = () =>
        {
            @event = events.an_event(random.event_log_sequence_number, random.aggregate_root_version);
        };

        Because of = () => result = @event.ToCommittedEvent();

        It should_return_a_committed_aggregate_event = () => result.ShouldBeOfExactType<CommittedAggregateEvent>();
        It should_represent_the_same_event = () => (result as CommittedAggregateEvent).ShouldBeTheSameAs(@event);
    }
}