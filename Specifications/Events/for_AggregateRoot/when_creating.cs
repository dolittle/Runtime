// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Specs.for_AggregateRoot.given;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_creating : given.two_aggregate_roots
    {
        static StatelessAggregateRoot aggregate_root;

        Because of = () =>
        {
            aggregate_root = new StatelessAggregateRoot(event_source_id);
        };

        It should_be_the_correct_event_source_id = () => aggregate_root.EventSource.ShouldEqual(event_source_id);
        It should_have_the_initial_version = () => aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
        It should_have_no_uncommitted_events = () => aggregate_root.UncommittedEvents.ShouldBeEmpty();
        It should_have_no_broken_rules = () => aggregate_root.BrokenRules.ShouldBeEmpty();
        It should_have_no_rule_set_evaulations = () => aggregate_root.RuleSetEvaluations.ShouldBeEmpty();
    }
}
