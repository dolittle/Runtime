// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;

namespace Dolittle.Runtime.Security
{
    /// <summary>
    /// Represents a principal resolver that resolves from current thread.
    /// </summary>
    public class DefaultPrincipalResolver : ICanResolvePrincipal
    {
        /// <summary>
        /// The user name when there is no user logged in.
        /// </summary>
        public const string AnonymousUserName = "[Anonymous]";

        /// <inheritdoc/>
        public ClaimsPrincipal Resolve()
        {
            if (ClaimsPrincipal.Current == null)
            {
                var identity = new ClaimsIdentity();
                identity.AddClaim(new System.Security.Claims.Claim(identity.NameClaimType, AnonymousUserName));
                var principal = new ClaimsPrincipal(identity);
                return principal;
            }

            return ClaimsPrincipal.Current;
        }
    }
}
