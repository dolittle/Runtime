// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Dolittle.Validation;
using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Specs.for_CommandResult
{
    public class when_containing_one_validation_result
    {
        static CommandResult result;
        static string error_message = "Something";

        Because of = () => result = new CommandResult
        {
            ValidationResults = new[]
            {
                new ValidationResult(error_message)
            }
        };

        It should_not_be_valid = () => result.Invalid.ShouldBeTrue();
        It should_not_be_successful = () => result.Success.ShouldBeFalse();

        It should_have_only_the_command_validation_message_in_all_validation_errors = () =>
                                                                                                {
                                                                                                    result.AllValidationMessages.Count().ShouldEqual(1);
                                                                                                    result.AllValidationMessages.First().ShouldEqual(error_message);
                                                                                                };
    }
}
