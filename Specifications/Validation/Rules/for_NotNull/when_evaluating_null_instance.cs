using System;
using doLittle.Rules;
using doLittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace doLittle.Specs.Validation.Rules.for_NotNull
{
    public class when_evaluating_null_instance
    {
        static NotNull rule;
        static Mock<IRuleContext> rule_context_mock;
        Establish context = () =>
        {
            rule = new NotNull(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, null);

        It should_fail_with_value_is_null_as_reason = () => rule_context_mock.Verify(r => r.Fail(rule, null, NotNull.ValueIsNull));
    }
}
