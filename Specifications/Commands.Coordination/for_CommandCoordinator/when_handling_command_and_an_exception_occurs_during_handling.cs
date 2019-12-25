// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Commands.Validation;
using Dolittle.Validation;
using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Coordination.Specs.for_CommandCoordinator
{
    [Subject(typeof(CommandCoordinator))]
    public class when_handling_command_and_an_exception_occurs_during_handling : given.a_command_coordinator
    {
        static CommandResult result;
        static Exception exception;

        Establish context = () =>
        {
            exception = new Exception();
            var validationResults = new CommandValidationResult { ValidationResults = Array.Empty<ValidationResult>() };
            command_validators_mock.Setup(cvs => cvs.Validate(command)).Returns(validationResults);
            command_handler_manager_mock.Setup(c => c.Handle(Moq.It.IsAny<CommandRequest>())).Throws(exception);
        };

        Because of = () => result = coordinator.Handle(command);

        It should_have_validated_the_command = () => command_validators_mock.VerifyAll();
        It should_have_authenticated_the_command = () => command_security_manager_mock.VerifyAll();
        It should_have_exception_in_result = () => result.Exception.ShouldEqual(exception);
        It should_have_success_set_to_false = () => result.Success.ShouldBeFalse();
    }
}