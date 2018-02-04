using doLittle.Rules;
using Machine.Specifications;

namespace doLittle.Queries.Validation.Specs.for_QueryArgumentValidationResult.given
{
    public class a_query_argument_validation_result_without_any_broken_rules
    {
        protected static QueryArgumentValidationResult result;

        Establish context = () => result = new QueryArgumentValidationResult(new BrokenRule[0]);
    }
}
