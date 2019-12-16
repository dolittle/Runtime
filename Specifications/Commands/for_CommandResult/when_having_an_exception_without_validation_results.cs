// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Runtime.Commands.Specs.for_CommandResult
{
    public class when_having_an_exception_without_validation_results
    {
        static CommandResult result;

        Because of = () => result = new CommandResult
        {
            Exception = new NotImplementedException()
        };

        It should_be_valid = () => result.Invalid.ShouldBeFalse();
        It should_not_be_successful = () => result.Success.ShouldBeFalse();
        It should_not_have_any_validation_messages = () => result.AllValidationMessages.ShouldBeEmpty();
    }
}
