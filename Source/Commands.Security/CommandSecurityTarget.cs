/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Security;
using doLittle.Commands;

namespace doLittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a specific <see cref="ISecurityTarget"/> for <see cref="ICommand">commands</see>
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
