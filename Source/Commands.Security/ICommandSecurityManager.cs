/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using doLittle.Commands;
using doLittle.Security;

namespace doLittle.Runtime.Commands.Security
{
    /// <summary>
    /// Defines a manager for dealing with security for <see cref="ICommand">commands</see>
    /// </summary>
    public interface ICommandSecurityManager
    {
        /// <summary>
        /// Authorizes a <see cref="CommandRequest"/> 
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/> to ask for</param>
        /// <returns><see cref="AuthorizationResult"/> that details how the <see cref="CommandRequest"/> was authorized</returns>
        AuthorizationResult Authorize(CommandRequest command);
    }
}
