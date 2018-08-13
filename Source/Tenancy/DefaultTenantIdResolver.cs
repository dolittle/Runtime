/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Linq;
using System.Security.Claims;

namespace Dolittle.Runtime.Tenancy
{
    /// <summary>
    /// Represents an implementation of <see cref="ICanResolveTenantId"/>
    /// </summary>
    /// <remarks>
    /// The resolver will look for the following claim types in the following order: 
    /// <see cref="TenantIdClaimType"/>
    /// <see cref="IdentityProviderClaimType"/>
    /// <see cref="ClaimTypes.Sid"/>
    /// 
    /// If not any of these claims are found to be identifying the tenant id, the <see cref="TenantId.Unknown"/>
    /// will be returned when resolving
    /// </remarks>
    public class DefaultTenantIdResolver : ICanResolveTenantId
    {
        /// <summary>
        /// The claim type representing TenantId
        /// </summary>
        public const string TenantIdClaimType = "http://schemas.microsoft.com/identity/claims/tenantid";

        /// <summary>
        /// The claim type representing the identity provider, which is often the tenant identifier
        /// </summary>
        public const string IdentityProviderClaimType = "http://schemas.microsoft.com/identity/claims/identityprovider";

        /// <inheritdoc/>
        public TenantId Resolve()
        {
            var principal = ClaimsPrincipal.Current;
            if (principal == null) return TenantId.Unknown;

            var claim = principal.Claims.FirstOrDefault(c => c.Type == TenantIdClaimType);
            if (claim == null) claim = principal.Claims.FirstOrDefault(c => c.Type == IdentityProviderClaimType);
            if (claim == null) claim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid);

            var tenantId = (claim != null) ? (TenantId)Guid.Parse(claim.Value) : TenantId.Unknown;
            return tenantId;
        }
    }
}
