// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Rules;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_LessThanOrEqual
{
    public class when_checking_value_that_is_equal_to
    {
        static double value = 42.0;
        static LessThanOrEqual<double> rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new LessThanOrEqual<double>(null, value);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, value);

        It should_not_fail = () => rule_context_mock.Verify(r => r.Fail(rule, value, Moq.It.IsAny<Cause>()), Times.Never());
    }
}
