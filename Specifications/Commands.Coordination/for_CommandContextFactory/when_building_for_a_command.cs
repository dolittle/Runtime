using Dolittle.Applications;
using Dolittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using System.Dynamic;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextFactory
{
    [Subject(typeof (CommandContextFactory))]
    public class when_building_for_a_command : given.a_command_context_factory
    {
        static CommandRequest command;
        static ICommandContext command_context;

        Establish context = () =>
            {
                command = new CommandRequest(TransactionCorrelationId.NotSet, Mock.Of<IApplicationArtifactIdentifier>(), new ExpandoObject());
            };

        Because of = () => command_context = factory.Build(command);

        It should_build_a_commmand_context = () => command_context.ShouldBeOfExactType<CommandContext>();
        It should_contain_the_commmand = () => command_context.Command.ShouldEqual(command);
    }
}