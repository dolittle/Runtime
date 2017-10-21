using doLittle.Read;

namespace doLittle.Specs.Read.Validation.for_QueryValidationDescriptorFor
{
    public class SomeQuery : IQueryFor<SomeReadModel>
    {
        public int IntegerArgument { get; set; }
    }
}
