// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Events.Specs.for_AggregateRoot
{
    public class when_applying_null : given.two_aggregate_roots
    {
        static Exception exception;

        Because of = () => exception = Catch.Exception(() =>
        {
            stateless_aggregate_root.Apply(null);
        });

        It should_throw_an_exception = () => exception.ShouldBeOfExactType<EventCanNotBeNull>();
        It should_not_increment_the_version = () => stateless_aggregate_root.Version.ShouldEqual(AggregateRootVersion.Initial);
        It should_have_no_uncommitted_events = () => stateless_aggregate_root.UncommittedEvents.ShouldBeEmpty();
        It should_have_no_broken_rules = () => statefull_aggregate_root.BrokenRules.ShouldBeEmpty();
        It should_have_no_rule_set_evaulations = () => statefull_aggregate_root.RuleSetEvaluations.ShouldBeEmpty();
    }
}
