using Dolittle.Rules;
using Machine.Specifications;

namespace Dolittle.Queries.Specs.for_QueryResult
{
    public class when_validation_has_broken_rules
    {
        static QueryResult result;

        Because of = () => result = new QueryResult { 
            BrokenRules = new[] { new BrokenRule(null, null, null) },
            Items = new object[0]
        };

        It should_be_considered_unsuccessful = () => result.Success.ShouldBeFalse();
        It should_be_invalid = () => result.Invalid.ShouldBeTrue();
    }
}
