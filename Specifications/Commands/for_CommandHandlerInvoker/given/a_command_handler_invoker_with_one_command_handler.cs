using doLittle.Runtime.Applications;
using doLittle.Commands;
using Machine.Specifications;
using Moq;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker.given
{
    public class a_command_handler_invoker_with_one_command_handler : a_command_handler_invoker_with_no_command_handlers
    {
        protected static CommandHandler handler;
        protected static ApplicationResourceIdentifier command_type;

        Establish context = () =>
                                {
                                    var application = new Mock<IApplication>();
                                    application.SetupGet(a => a.Name).Returns("An Application");
                                    var applicationResource = new Mock<IApplicationResource>();
                                    applicationResource.SetupGet(a => a.Name).Returns("A Resource");
                                    command_type = new ApplicationResourceIdentifier(application.Object, new IApplicationLocation[0], applicationResource.Object);
                                    application_resources.Setup(a => a.Identify(typeof(Command))).Returns(command_type);
                                    handler = new CommandHandler();
                                    type_finder.Setup(t => t.FindMultiple<ICanHandleCommands>()).Returns(new[]
                                                                                                              {typeof(CommandHandler)});

                                    container.Setup(c => c.Get(typeof (CommandHandler))).Returns(handler);
                                };
    }
}