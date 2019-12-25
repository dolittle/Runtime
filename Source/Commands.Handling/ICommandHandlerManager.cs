// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Commands.Handling
{
    /// <summary>
    /// Defines the functionality for a manager that handles commands.
    /// </summary>
    /// <remarks>
    /// Handles a <see cref="CommandRequest">command</see> by calling any
    /// command handlers that can handle the specific command.
    /// </remarks>
    public interface ICommandHandlerManager
    {
        /// <summary>
        /// Handle a command.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest">Command</see> to handle.</param>
        void Handle(CommandRequest command);
    }
}