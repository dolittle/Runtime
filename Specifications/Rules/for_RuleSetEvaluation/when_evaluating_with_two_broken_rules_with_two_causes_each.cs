// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rules.for_RuleSetEvaluation
{
    public class when_evaluating_with_two_broken_rules_with_two_causes_each
    {
        static Reason first_rule_first_reason = Reason.Create("e8a91403-35f1-43e9-ab07-ddc677d4dddd", "First Rule First Reason");
        static Reason first_rule_second_reason = Reason.Create("3ea2a5ff-6d32-4211-806a-4fd5bada6a5a", "First Rule Second Reason");
        static Reason second_rule_first_reason = Reason.Create("e5179c37-b57d-422d-8d8f-4d754b833702", "Second Rule First Reason");
        static Reason second_rule_second_reason = Reason.Create("e2a01773-33db-457b-91e5-83e9563417e7", "Second Rule Second Reason");
        static Cause first_rule_first_cause;
        static Cause first_rule_second_cause;
        static Cause second_rule_first_cause;
        static Cause second_rule_second_cause;

        static Mock<IRule> first_rule;
        static Mock<IRule> second_rule;
        static RuleSetEvaluation evaluation;

        Establish context = () =>
        {
            first_rule_first_cause = first_rule_first_reason.NoArgs();
            first_rule_second_cause = first_rule_second_reason.NoArgs();
            second_rule_first_cause = second_rule_first_reason.NoArgs();
            second_rule_second_cause = second_rule_second_reason.NoArgs();

            first_rule = new Mock<IRule>();
            first_rule.Setup(_ => _.Evaluate(Moq.It.IsAny<IRuleContext>(), Moq.It.IsAny<object>())).Callback(
                (IRuleContext context, object target) =>
                {
                    context.Fail(first_rule.Object, target, first_rule_first_cause);
                    context.Fail(first_rule.Object, target, first_rule_second_cause);
                });

            second_rule = new Mock<IRule>();
            second_rule.Setup(_ => _.Evaluate(Moq.It.IsAny<IRuleContext>(), Moq.It.IsAny<object>())).Callback(
                (IRuleContext context, object target) =>
                {
                    context.Fail(second_rule.Object, target, second_rule_first_cause);
                    context.Fail(second_rule.Object, target, second_rule_second_cause);
                });

            evaluation = new RuleSetEvaluation(new RuleSet(new IRule[]
            {
                first_rule.Object,
                second_rule.Object
            }));
        };

        Because of = () => evaluation.Evaluate(new object());

        It should_have_two_broken_rules = () => evaluation.BrokenRules.Count().ShouldEqual(2);

        It should_have_the_first_rule_broken = () => evaluation.BrokenRules.ToArray()[0].Rule.ShouldEqual(first_rule.Object);
        It should_have_the_first_rule_broken_with_first_reason = () => evaluation.BrokenRules.ToArray()[0].Causes.ToArray()[0].ShouldEqual(first_rule_first_cause);
        It should_have_the_first_rule_broken_with_second_reason = () => evaluation.BrokenRules.ToArray()[0].Causes.ToArray()[1].ShouldEqual(first_rule_second_cause);

        It should_have_the_second_rule_broken = () => evaluation.BrokenRules.ToArray()[1].Rule.ShouldEqual(second_rule.Object);
        It should_have_the_second_rule_broken_with_first_reason = () => evaluation.BrokenRules.ToArray()[1].Causes.ToArray()[0].ShouldEqual(second_rule_first_cause);
        It should_have_the_second_rule_broken_with_second_reason = () => evaluation.BrokenRules.ToArray()[1].Causes.ToArray()[1].ShouldEqual(second_rule_second_cause);

        It should_be_considered_unsuccessful = () => evaluation.IsSuccess.ShouldBeFalse();
        It should_be_considered_unsuccessful_through_implicit_operator = () => ((bool)evaluation).ShouldBeFalse();
    }
}