using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandContextManager
{
    [Subject(Subjects.getting_context)]
    public class when_getting_current : given.a_command_context_manager
    {
        static ICommandContext commandContext;
        static ICommandContext currentContext;
        static CommandRequest command;

        Establish context = () =>
                                {
                                    var artifact = Artifact.New();
                                    command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
                                    commandContext = Manager.EstablishForCommand(command);
                                };

        Because of = () => currentContext = Manager.GetCurrent();

        It should_not_return_null = () => currentContext.ShouldNotBeNull();
        It should_return_same_context_as_when_calling_it_for_a_command = () => currentContext.ShouldEqual(commandContext);
        It should_return_context_with_command_in_it = () => currentContext.Command.ShouldEqual(command);
        It should_return_same_when_calling_it_twice_on_same_thread = () =>
                                                                         {
                                                                             var secondContext = Manager.GetCurrent();
                                                                             secondContext.ShouldEqual(currentContext);
                                                                         };
    }
}