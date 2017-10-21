using System.Dynamic;
using doLittle.Runtime.Applications;
using doLittle.Commands;
using doLittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker
{
    [Subject(Subjects.handling_commands)]
    public class when_handling_with_manually_registered_command_handlers : given.a_command_handler_invoker_with_no_command_handlers
    {
        static CommandHandler handler;
        static CommandRequest command;
        static ICommand command_instance;
        static IApplicationResourceIdentifier command_type;
        static bool result;

        Establish context = () =>
        {
            var application = new Mock<IApplication>();
            application.SetupGet(a => a.Name).Returns("An Application");
            var applicationResource = new Mock<IApplicationResource>();
            applicationResource.SetupGet(a => a.Name).Returns("A Resource");
            command_type = new ApplicationResourceIdentifier(application.Object, new IApplicationLocation[0], applicationResource.Object);
            application_resources.Setup(a => a.Identify(typeof(Command))).Returns(command_type);

            command = new CommandRequest(TransactionCorrelationId.NotSet, command_type, new ExpandoObject());
            command_instance = new Command();
            command_request_converter.Setup(c => c.Convert(command)).Returns(command_instance);

            handler = new CommandHandler();
            container.Setup(c => c.Get(typeof(CommandHandler))).Returns(handler);
            invoker.Register(typeof(CommandHandler));
        };

        Because of = () => result = invoker.TryHandle(command);

        It should_return_true_when_trying_to_handle = () => result.ShouldBeTrue();
    }
}
