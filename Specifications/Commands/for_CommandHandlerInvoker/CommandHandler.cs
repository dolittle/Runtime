namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker
{
    public class CommandHandler : ICanHandleCommands
    {
        public bool HandleCalled = false;


        public void Handle(Command command)
        {
            HandleCalled = true;
        }
    }
}
