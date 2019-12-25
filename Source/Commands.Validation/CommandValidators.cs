// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Types;
using Dolittle.Validation;

namespace Dolittle.Runtime.Commands.Validation
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommandValidators"/>.
    /// </summary>
    public class CommandValidators : ICommandValidators
    {
        readonly IInstancesOf<ICommandValidator> _validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandValidators"/> class.
        /// </summary>
        /// <param name="validators">Instances of validators to use.</param>
        public CommandValidators(IInstancesOf<ICommandValidator> validators)
        {
            _validators = validators;
        }

        /// <inheritdoc/>
        public CommandValidationResult Validate(CommandRequest command)
        {
            var errorMessages = new List<string>();
            var validationResults = new List<ValidationResult>();

            foreach (var validator in _validators)
            {
                var validatorResult = validator.Validate(command);
                errorMessages.AddRange(validatorResult.CommandErrorMessages);
                validationResults.AddRange(validatorResult.ValidationResults);
            }

            var result = new CommandValidationResult
            {
                CommandErrorMessages = errorMessages,
                ValidationResults = validationResults
            };
            return result;
        }
    }
}
