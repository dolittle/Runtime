// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Defines a resolver for <see cref="ClaimsPrincipal"/>.
    /// </summary>
    /// <remarks>
    /// An application may implement this convention once. If it is not implemented,
    /// the <see cref="DefaultPrincipalResolver"/> is used.
    /// </remarks>
    public interface ICanResolvePrincipal
    {
        /// <summary>
        /// Method that is called to resolve current <see cref="ClaimsPrincipal"/>.
        /// </summary>
        /// <returns>The resolved <see cref="ClaimsPrincipal"/>.</returns>
        ClaimsPrincipal Resolve();
    }
}
