using Machine.Specifications;

namespace doLittle.Queries.Validation.Specs.for_QueryValidationDescriptorFor.given
{
    public class an_empty_query_validation_descriptor
    {
        protected static QueryValidationDescriptorFor<SomeQuery> descriptor;

        Establish context = () => descriptor = new QueryValidationDescriptorFor<SomeQuery>();
    }
}
