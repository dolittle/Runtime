using System;
using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Runtime.Commands.Handling;
using Dolittle.Runtime.Transactions;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Specs.for_CommandHandlerManager
{
    public class when_handling_a_command_without_a_command_handler : given.a_command_handler_manager
    {
        static Exception thrown_exception;
        static CommandRequest handled_command;

        Because of = () =>
                         {
                             handled_command = new CommandRequest(TransactionCorrelationId.NotSet, Artifact.New(), new ExpandoObject());
                             thrown_exception = Catch.Exception(() => manager.Handle(handled_command));
                         };

        It should_throw_unhandled_command_exception = () => thrown_exception.ShouldBeOfExactType<CommandWasNotHandled>();
    }
}
