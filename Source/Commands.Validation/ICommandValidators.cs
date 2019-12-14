// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Commands.Validation
{
    /// <summary>
    /// Defines the validators for validating a <see cref="CommandRequest"/>.
    /// </summary>
    public interface ICommandValidators
    {
        /// <summary>
        /// Validate the command.
        /// </summary>
        /// <param name="command">Instance to be validated.</param>
        /// <returns>Validation results for a <see cref="CommandRequest">Command</see>.</returns>
        CommandValidationResult Validate(CommandRequest command);
    }
}
