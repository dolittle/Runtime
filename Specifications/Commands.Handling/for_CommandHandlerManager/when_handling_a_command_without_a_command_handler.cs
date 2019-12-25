// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Runtime.Commands.Handling;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Specs.for_CommandHandlerManager
{
    public class when_handling_a_command_without_a_command_handler : given.a_command_handler_manager
    {
        static Exception thrown_exception;
        static CommandRequest handled_command;

        Because of = () =>
                         {
                             var artifact = Artifact.New();
                             handled_command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
                             thrown_exception = Catch.Exception(() => manager.Handle(handled_command));
                         };

        It should_throw_unhandled_command_exception = () => thrown_exception.ShouldBeOfExactType<CommandWasNotHandled>();
    }
}
