// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Store.Specs.for_CommittedAggregateEvents
{
    public class when_creating_with_events : given.events_and_an_artifact
    {
        static CommittedAggregateEvents events;

        Because of = () => events = new CommittedAggregateEvents(event_source_id, aggregate_artifact.Id, aggregate_version_before, aggregate_version_after, new[] { event_one, event_two, event_three });

        It should_not_be_empty = () => events.ShouldNotBeEmpty();
        It should_have_events = () => events.HasEvents.ShouldBeTrue();
        It should_have_a_count_of_three = () => events.Count.ShouldEqual(3);
        It should_have_the_first_event_at_index_zero = () => events[0].ShouldEqual(event_one);
        It should_have_the_second_event_at_index_one = () => events[1].ShouldEqual(event_two);
        It should_have_the_third_event_at_index_two = () => events[2].ShouldEqual(event_three);
    }
}
