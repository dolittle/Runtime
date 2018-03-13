/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace Dolittle.Runtime.Commands.Handling
{
    /// <summary>
    /// Defines the functionality for a manager that handles commands
    /// 
    /// Handles a <see cref="CommandRequest">command</see> by calling any
    /// command handlers that can handle the specific command
    /// </summary>
    public interface ICommandHandlerManager
    {
        /// <summary>
        /// Handle a command
        /// </summary>
        /// <param name="command"><see cref="CommandRequest">Command</see> to handle</param>
        void Handle(CommandRequest command);
    }
}