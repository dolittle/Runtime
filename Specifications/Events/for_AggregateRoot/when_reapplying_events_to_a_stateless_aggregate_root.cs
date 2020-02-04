// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Specs.for_AggregateRoot.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_reapplying_events_to_a_stateless_aggregate_root : given.committed_events_and_two_aggregate_roots
    {
        static CommittedAggregateEvents events;

        Establish context = () =>
        {
            events = build_committed_events(event_source_id, typeof(StatelessAggregateRoot));
        };

        Because of = () =>
        {
            stateless_aggregate_root.ReApply(events);
        };

        It should_be_at_version_three = () => stateless_aggregate_root.Version.ShouldEqual((AggregateRootVersion)3);
        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
        It should_have_no_broken_rules = () => stateless_aggregate_root.BrokenRules.ShouldBeEmpty();
        It should_have_no_rule_set_evaulations = () => stateless_aggregate_root.RuleSetEvaluations.ShouldBeEmpty();
    }
}
