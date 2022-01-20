// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Security.Claims;

namespace Dolittle.Runtime.Security;

/// <summary>
/// Extensions related to <see cref="ClaimsPrincipal"/>.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    ///  Creates a <see cref="Claims"/> instance from a Claims Principal.
    /// </summary>
    /// <param name="claimsPrincipal">The <see cref="ClaimsPrincipal"/> to convert from.</param>
    /// <returns>a <see cref="Claims"/> instance.</returns>
    public static Claims ToClaims(this ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal == null)
            return null;

        return new Claims(claimsPrincipal.Claims.Select(c => new Claim(c.Type, c.Value, c.ValueType)));
    }

    /// <summary>
    ///  Creates a <see cref="ClaimsPrincipal"/> instance from a <see cref="Claims" /> instance.
    /// </summary>
    /// <param name="claims"><see cref="Claims"/> to convert from.</param>
    /// <returns>a <see cref="ClaimsPrincipal"/> instance.</returns>
    public static ClaimsPrincipal ToClaimsPrincipal(this Claims claims)
    {
        if (claims == null)
            return new ClaimsPrincipal();

        return new ClaimsPrincipal(new ClaimsIdentity(claims.Select(c => c.ToDotnetClaim())));
    }
}