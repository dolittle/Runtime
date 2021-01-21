// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rules.for_BrokenRule
{
    public class when_adding_two_causes
    {
        static Mock<IRule> rule;
        static Mock<IRuleContext> rule_context;
        static object instance;
        static BrokenRule broken_rule;
        static Reason first_reason = Reason.Create("2da82a45-e779-4823-ae12-c02023ee8e5f", "First reason");
        static Reason second_reason = Reason.Create("5e536622-5b92-44ea-ba83-cfa14d5029b4", "Second reason");

        static Cause first_cause;
        static Cause second_cause;

        Establish context = () =>
        {
            rule = new Mock<IRule>();
            rule_context = new Mock<IRuleContext>();
            instance = new object();
            broken_rule = new BrokenRule(rule.Object, instance, rule_context.Object);

            first_cause = first_reason.NoArgs();
            second_cause = second_reason.NoArgs();
        };

        Because of = () =>
        {
            broken_rule.AddCause(first_cause);
            broken_rule.AddCause(second_cause);
        };

        It should_contain_only_the_added_causes = () => broken_rule.Causes.ShouldContainOnly(first_cause, second_cause);
    }
}