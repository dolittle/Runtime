using System.Dynamic;
using doLittle.Runtime.Applications;
using doLittle.Commands;
using doLittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace doLittle.Runtime.Commands.Specs.for_CommandContextManager
{
    [Subject(Subjects.establishing_context)]
    public class when_establishing_for_same_command : given.a_command_context_manager
    {
        static ICommandContext commandContext;
        static CommandRequest command;

        Because of = () =>
                         {
                             command = new CommandRequest(TransactionCorrelationId.NotSet, Mock.Of<IApplicationResourceIdentifier>(), new ExpandoObject());
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
