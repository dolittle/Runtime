// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;

namespace Dolittle.Runtime.Commands.Security
{
    /// <summary>
    /// Defines a manager for dealing with security for <see cref="CommandRequest">commands</see>.
    /// </summary>
    public interface ICommandSecurityManager
    {
        /// <summary>
        /// Authorizes a <see cref="CommandRequest"/>.
        /// </summary>
        /// <param name="command"><see cref="CommandRequest"/> to ask for.</param>
        /// <returns><see cref="AuthorizationResult"/> that details how the <see cref="CommandRequest"/> was authorized.</returns>
        AuthorizationResult Authorize(CommandRequest command);
    }
}
