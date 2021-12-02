// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Principal;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Extensions for <see cref="ISecurable"/>.
/// </summary>
public static class SecurableExtensions
{
    /// <summary>
    /// Define a user actor for a <see cref="ISecurable">securable</see>.
    /// </summary>
    /// <param name="securable"><see cref="ISecurable"/> to secure.</param>
    /// <param name="principalResolver">Resolves the <see cref="IPrincipal" />.</param>
    /// <returns>The <see cref="ISecurable"/> chain.</returns>
    public static UserSecurityActor UserFrom(this ISecurable securable, ICanResolvePrincipal principalResolver)
    {
        var actor = new UserSecurityActor(principalResolver);
        securable.AddActor(actor);
        return actor;
    }
}