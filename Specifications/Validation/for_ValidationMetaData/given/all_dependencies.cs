using Dolittle.Types;
using Dolittle.Validation.MetaData;
using Machine.Specifications;
using Moq;

namespace Dolittle.Specs.Validation.for_ValidationMetaData.given
{
    public class all_dependencies
    {
        protected static Mock<IInstancesOf<ICanGenerateValidationMetaData>> generators_mock;

        Establish context = () => generators_mock = new Mock<IInstancesOf<ICanGenerateValidationMetaData>>();
    }
}
