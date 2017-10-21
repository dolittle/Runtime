using doLittle.Commands;
using Machine.Specifications;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker.given
{
    public class a_command_handler_invoker_with_no_command_handlers : all_dependencies
    {
        protected static CommandHandlerInvoker invoker;

        Establish context = () => invoker = new CommandHandlerInvoker(type_finder.Object, container.Object, application_resources.Object, command_request_converter.Object, logger.Object); 
    }
}
