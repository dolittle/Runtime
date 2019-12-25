// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Commands.Handling
{
    /// <summary>
    /// Invokes a command for a command handler type.
    /// </summary>
    public interface ICommandHandlerInvoker
    {
        /// <summary>
        /// Try to handle a command.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest">Command</see> to invoke handler for.</param>
        /// <returns>True if it handled it, false if not.</returns>
        /// <remarks>
        /// If it can handle it, it should handle it - and return true.
        /// if it handled it and false if not.
        /// </remarks>
        bool TryHandle(CommandRequest command);
    }
}