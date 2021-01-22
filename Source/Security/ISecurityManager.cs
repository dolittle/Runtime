// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Defines a manager for dealing with security for types and namespaces.
    /// </summary>
    public interface ISecurityManager
    {
        /// <summary>
        /// Ask if an instance of a action is authorized.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="ISecurityAction"/> that we with to authorize.</typeparam>
        /// <param name="target">Object that is subject of security.</param>
        /// <returns><see cref="AuthorizationResult"/> that contains the result.</returns>
        AuthorizationResult Authorize<T>(object target)
            where T : ISecurityAction;
    }
}
