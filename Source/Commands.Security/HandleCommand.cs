/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Security;
using doLittle.Commands;

namespace doLittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for handling <see cref="ICommand">commands</see>
    /// </summary>
    public class HandleCommand : SecurityAction
    {
        /// <inheritdoc/>
        public override string ActionType
        {
            get { return "Handle"; }
        }
    }
}
