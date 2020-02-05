// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_fast_forwarding_a_stateless_aggregate_root : given.two_aggregate_roots
    {
        Because of = () =>
        {
            stateless_aggregate_root.FastForward(2);
        };

        It should_be_at_the_correct_version = () => stateless_aggregate_root.Version.ShouldEqual((AggregateRootVersion)2);
        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
        It should_have_no_broken_rules = () => stateless_aggregate_root.BrokenRules.ShouldBeEmpty();
        It should_have_no_rule_set_evaulations = () => stateless_aggregate_root.RuleSetEvaluations.ShouldBeEmpty();
    }
}
