using Dolittle.Rules;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Validation.Specs.for_QueryValidationResult.given
{
    public class all_dependencies
    {
        protected static Mock<IRule> rule_mock;
        protected static object instance;
        protected static Mock<IRuleContext> rule_context_mock;

        Establish context = () =>
        {
            rule_mock = new Mock<IRule>();
            instance = new object();
            rule_context_mock = new Mock<IRuleContext>();
        };
    }
}
