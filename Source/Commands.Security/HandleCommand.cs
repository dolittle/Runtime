/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Security;
using Dolittle.Runtime.Commands;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents a <see cref="ISecurityAction"/> for handling <see cref="CommandRequest">commands</see>
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
