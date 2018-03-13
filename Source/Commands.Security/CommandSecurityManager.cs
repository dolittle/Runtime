/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using Dolittle.Security;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Represents an implementation of <see cref="ICommandSecurityManager"/>
    /// </summary>
    public class CommandSecurityManager : ICommandSecurityManager
    {
        ISecurityManager _securityManager;

        /// <summary>
        /// Initializes a new instance of <see cref="CommandSecurityManager"/>
        /// </summary>
        /// <param name="securityManager"><see cref="ISecurityManager"/> for forwarding requests related to security to</param>
        public CommandSecurityManager(ISecurityManager securityManager)
        {
            _securityManager = securityManager;
        }

        /// <inheritdoc/>
        public AuthorizationResult Authorize(CommandRequest command)
        {
            return _securityManager.Authorize<HandleCommand>(command);
        }
    }
}
