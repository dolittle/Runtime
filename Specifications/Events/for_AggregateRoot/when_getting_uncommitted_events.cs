// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Specs.for_AggregateRoot.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_getting_uncommitted_events : given.two_aggregate_roots
    {
        static UncommittedAggregateEvents events;

        Establish context = () =>
        {
            stateless_aggregate_root.Apply(event_one);
            stateless_aggregate_root.Apply(event_two);
            stateless_aggregate_root.Apply(event_three);
        };

        Because of = () =>
        {
            events = stateless_aggregate_root.UncommittedEvents;
        };

        It should_have_three_events = () => events.Count.ShouldEqual(3);
        It should_be_applied_to_the_correct_event_source = () => events.EventSource.ShouldEqual(event_source_id);
        It should_be_applied_by_the_correct_aggregate_root_type = () => events.AggregateRoot.ShouldEqual(typeof(StatelessAggregateRoot));
        It should_expect_the_correct_aggregate_root_version = () => events.ExpectedAggregateRootVersion.ShouldEqual(AggregateRootVersion.Initial);
        It should_have_event_one_at_index_zero = () => events[0].ShouldEqual(event_one);
        It should_have_event_two_at_index_one = () => events[1].ShouldEqual(event_two);
        It should_have_event_three_at_index_two = () => events[2].ShouldEqual(event_three);
    }
}
