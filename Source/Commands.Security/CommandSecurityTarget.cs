/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Security;
using Dolittle.Runtime.Commands;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a specific <see cref="ISecurityTarget"/> for <see cref="CommandRequest">commands</see>
    /// </summary>
    public class CommandSecurityTarget : SecurityTarget
    {
        const string COMMAND = "Command";

        /// <summary>
        /// Instantiates an instance of <see cref="CommandSecurityTarget"/>
        /// </summary>
        public CommandSecurityTarget() : base(COMMAND)
        {
        }
    }
}
