// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Commands.Handling
{
    /// <summary>
    /// Exception that gets thrown when a command is not handled by any handlers.
    /// </summary>
    public class CommandWasNotHandled : ArgumentException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommandWasNotHandled"/> class.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/> that wasn't handled.</param>
        public CommandWasNotHandled(CommandRequest command)
            : base($"Command of type '{command.Type}' was not handled")
        {
        }
    }
}
