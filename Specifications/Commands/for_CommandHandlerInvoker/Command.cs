using System;
using doLittle.Commands;

namespace doLittle.Runtime.Commands.Specs.for_CommandHandlerInvoker
{
    public class Command : ICommand
    {
        public Guid Id { get; set; }
    }
}
