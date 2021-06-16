// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Machine.Specifications;

namespace Dolittle.Runtime.Security.for_ClaimsPrincipalExtensions.given
{
    public class a_claims_principal
    {
        public static IEnumerable<Claim> dolittle_claims;
        public static IEnumerable<System.Security.Claims.Claim> dotnet_claims;
        public static ClaimsPrincipal instance;

        Establish context = () =>
        {
            dotnet_claims = new List<System.Security.Claims.Claim>
            {
                new System.Security.Claims.Claim(ClaimTypes.Name, "Michael"),
                new System.Security.Claims.Claim(ClaimTypes.Country, "Norway"),
                new System.Security.Claims.Claim(ClaimTypes.Gender, "M"),
                new System.Security.Claims.Claim(ClaimTypes.Surname, "Smith"),
                new System.Security.Claims.Claim(ClaimTypes.Email, "michael@dolittle.com"),
                new System.Security.Claims.Claim(ClaimTypes.Role, "Coffee Maker")
            };

            dolittle_claims = dotnet_claims.Select(Claim.FromDotnetClaim);

            instance = new ClaimsPrincipal(new ClaimsIdentity(dotnet_claims));
        };
    }
}