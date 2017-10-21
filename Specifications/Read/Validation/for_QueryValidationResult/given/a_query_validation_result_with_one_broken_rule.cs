using doLittle.Read.Validation;
using doLittle.Rules;
using Machine.Specifications;
using Moq;

namespace doLittle.Specs.Read.Validation.for_QueryValidationResult.given
{
    public class a_query_validation_result_with_one_broken_rule : all_dependencies
    {
        protected static QueryValidationResult result;


        Establish context = () => result = new QueryValidationResult(new [] {new BrokenRule(rule_mock.Object, instance, rule_context_mock.Object)});
    }
}
