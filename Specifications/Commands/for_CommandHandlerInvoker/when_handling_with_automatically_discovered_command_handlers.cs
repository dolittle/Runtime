using System.Dynamic;
using doLittle.Commands;
using doLittle.Runtime.Transactions;
using Machine.Specifications;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker
{
    [Subject(Subjects.handling_commands)]
    public class when_handling_with_automatically_discovered_command_handlers : given.a_command_handler_invoker_with_one_command_handler
    {
        static bool result;
        static CommandRequest command;
        static ICommand command_instance;

        Establish context = () =>
        {
            command = new CommandRequest(TransactionCorrelationId.NotSet, command_type, new ExpandoObject());
            command_instance = new Command();
            command_request_converter.Setup(c => c.Convert(command)).Returns(command_instance);
        };

        Because of = () => result = invoker.TryHandle(command);

        It should_return_true_when_trying_to_handle = () => result.ShouldBeTrue();
    }
}