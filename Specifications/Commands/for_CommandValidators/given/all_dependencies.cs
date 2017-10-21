using doLittle.Commands;
using doLittle.Execution;
using doLittle.Types;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Commands.Specs.for_CommandValidators.given
{
    public class all_dependencies
    {
        protected static Mock<IInstancesOf<ICommandValidator>> validators_mock;

        Establish context = () => validators_mock = new Mock<IInstancesOf<ICommandValidator>>();
    }
}
