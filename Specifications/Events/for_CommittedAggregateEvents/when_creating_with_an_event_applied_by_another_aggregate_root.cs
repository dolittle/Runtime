// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_CommittedAggregateEvents
{
    public class when_creating_with_an_event_applied_by_another_aggregate_root : given.an_aggregate_instance_and_some_committed_events
    {
        static CommittedAggregateEvents events;
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            events = new CommittedAggregateEvents(event_source_id, aggregate_root_type, aggregate_root_version, new CommittedAggregateEvent[] { first_event, second_event, event_with_other_aggregate_root_type });
        });

        It should_not_be_created = () => events.ShouldBeNull();
        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventWasAppliedByOtherAggregateRoot>();
    }
}
