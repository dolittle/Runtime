/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
namespace doLittle.Runtime.Commands.Handling
{
    /// <summary>
    /// Invokes a command for a command handler type
    /// </summary>
    public interface ICommandHandlerInvoker
    {
        /// <summary>
        /// Try to handle a command
        /// 
        /// If it can handle it, it should handle it - and return true
        /// if it handled it and false if not
        /// </summary>
        /// <param name="command"><see cref="CommandRequest">Command</see> to invoke handler for</param>
        /// <returns>True if it handled it, false if not</returns>
        bool TryHandle(CommandRequest command);
    }
}