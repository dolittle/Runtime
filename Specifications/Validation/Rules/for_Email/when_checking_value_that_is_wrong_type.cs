using Dolittle.Machine.Specifications.Rules;
using Dolittle.Rules;
using Dolittle.Validation;
using Dolittle.Validation.Rules;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Specs.Validation.Rules.for_Email
{
    public class when_checking_value_that_is_wrong_type
    {
        static Email rule;
        static Mock<IRuleContext> rule_context_mock;

        Establish context = () => 
        {
            rule = new Email(null);
            rule_context_mock = new Mock<IRuleContext>();
        };

        Because of = () => rule.Evaluate(rule_context_mock.Object, 42);

        It should_fail_with_wrong_type_as_reason = () => rule_context_mock.ShouldFailWith(rule, Moq.It.IsAny<object>(), ValueRule.ValueTypeMismatch);
    }
}
