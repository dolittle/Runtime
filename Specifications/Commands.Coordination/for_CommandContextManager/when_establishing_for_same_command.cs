using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextManager
{
    [Subject(Subjects.establishing_context)]
    public class when_establishing_for_same_command : given.a_command_context_manager
    {
        static ICommandContext commandContext;
        static CommandRequest command;

        Because of = () =>
                         {
                             var artifact = Artifact.New();
                             command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
                             commandContext = Manager.EstablishForCommand(command);
                         };

        It should_return_a_non_null_context = () => commandContext.ShouldNotBeNull();
        It should_return_context_with_command_in_it = () => commandContext.Command.ShouldEqual(command);
        It should_return_the_same_calling_it_twice_on_same_thread = () =>
                                                                        {
                                                                            var secondContext = Manager.EstablishForCommand(command);
                                                                            secondContext.ShouldEqual(commandContext);
                                                                        };
    }
}
