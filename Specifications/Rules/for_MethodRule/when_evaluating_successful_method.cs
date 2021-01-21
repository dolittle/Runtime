// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Rules.for_MethodRule
{
    public class when_evaluating_successful_method
    {
        static RuleEvaluationResult RuleMethod()
        {
            return RuleEvaluationResult.Success;
        }

        const string rule_name = "This is the rule";

        static Mock<IRuleContext> rule_context;
        static object instance;
        static MethodRule rule;

        Establish context = () =>
        {
            rule_context = new Mock<IRuleContext>();
            instance = new object();
            rule = new MethodRule(rule_name, RuleMethod);
        };

        Because of = () => rule.Evaluate(rule_context.Object, instance);

        It should_not_any_failures = () => rule_context.Verify(_ => _.Fail(Moq.It.IsAny<IRule>(), Moq.It.IsAny<object>(), Moq.It.IsAny<Cause>()), Moq.Times.Never);
    }
}