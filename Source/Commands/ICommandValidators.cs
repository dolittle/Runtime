/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Runtime.Commands
{
    /// <summary>
    /// Defines the validators for validating a <see cref="CommandRequest"/>
    /// </summary>
    public interface ICommandValidators
    {
        /// <summary>
        /// Validate the command
        /// </summary>
        /// <param name="command">Instance to be validated</param>
        /// <returns>Validation results for a <see cref="CommandRequest">Command</see></returns>
        CommandValidationResult Validate(CommandRequest command);
    }
}
