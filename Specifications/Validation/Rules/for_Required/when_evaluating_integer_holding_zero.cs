using System;
using Dolittle.Rules;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_Required
{
    public class when_evaluating_integer_holding_zero
    {
        static Required rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule = new Required(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, 0);

        It should_fail_with_value_not_specified_as_reason = () => rule_context_mock.Verify(r => r.Fail(rule, 0, Required.ValueNotSpecified));
    }
}
