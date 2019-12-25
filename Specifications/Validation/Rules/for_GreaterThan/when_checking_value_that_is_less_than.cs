// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_GreaterThan
{
    public class when_checking_value_that_is_less_than
    {
        static double value = 41.0;
        static GreaterThan<double> rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new GreaterThan<double>(null, 42.0);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, value);

        It should_fail_with_value_is_less_than_reason = () => rule_context_mock.Verify(r => r.Fail(rule, value, Moq.It.Is<Cause>(_ => _.Reason == Reasons.ValueIsLessThan)), Times.Once());
    }
}
