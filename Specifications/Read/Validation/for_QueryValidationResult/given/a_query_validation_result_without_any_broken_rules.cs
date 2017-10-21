using doLittle.Read.Validation;
using doLittle.Rules;
using Machine.Specifications;

namespace doLittle.Specs.Read.Validation.for_QueryValidationResult.given
{
    public class a_query_validation_result_without_any_broken_rules
    {
        protected static QueryValidationResult result;

        Establish context = () => result = new QueryValidationResult(new BrokenRule[0]);
    }
}
