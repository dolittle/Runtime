// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Dynamic;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Validation.Specs.for_CommandValidators
{
    public class when_validating_and_there_are_two_providers : given.command_validators_with_two_providers
    {
        static CommandRequest command;
        static CommandValidationResult result;

        Establish context = () =>
        {
            var artifact = Artifact.New();
            command = new CommandRequest(CorrelationId.Empty, artifact.Id, artifact.Generation, new ExpandoObject());
        };

        Because of = () => result = validators.Validate(command);

        It should_return_a_result = () => result.ShouldNotBeNull();

        It should_validate_using_the_first_validator = () => first_validator.validate_called.ShouldBeTrue();
        It should_hold_the_error_message_from_the_first_validator = () => result.CommandErrorMessages.ShouldContain(first_validator_command_error_message);
        It should_hold_the_validation_result_from_the_first_validator = () => result.ValidationResults.ShouldContain(first_validator_validation_result);

        It should_validate_using_the_second_validator = () => second_validator.validate_called.ShouldBeTrue();
        It should_hold_the_error_message_from_the_second_validator = () => result.CommandErrorMessages.ShouldContain(second_validator_command_error_message);
        It should_hold_the_validation_result_from_the_second_validator = () => result.ValidationResults.ShouldContain(second_validator_validation_result);
    }
}
