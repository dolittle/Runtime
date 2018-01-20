/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;

namespace doLittle.Runtime.Commands
{
    /// <summary>
    /// The exception that is thrown when a command is not handled by any handlers
    /// </summary>
    public class CommandWasNotHandled : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance <see cref="CommandWasNotHandled"/>
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/> that wasn't handled</param>
        public CommandWasNotHandled(CommandRequest command) : base(string.Format("Command of type '{0}' was not handled",command.Type))
        {
        }
    }
}
