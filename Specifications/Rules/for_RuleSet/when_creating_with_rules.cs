// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rules.for_RuleSet
{
    public class when_creating_with_rules
    {
        static IRule first_rule;
        static IRule second_rule;

        static RuleSet rule_set;

        Establish context = () =>
        {
            first_rule = Mock.Of<IRule>();
            second_rule = Mock.Of<IRule>();
        };

        Because of = () => rule_set = new RuleSet(new IRule[] { first_rule, second_rule });

        It should_hold_only_the_rules_passed = () => rule_set.Rules.ShouldContainOnly(first_rule, second_rule);
    }
}