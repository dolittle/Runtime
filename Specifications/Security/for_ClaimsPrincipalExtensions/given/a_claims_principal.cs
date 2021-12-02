// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Machine.Specifications;

namespace Dolittle.Runtime.Security.for_ClaimsPrincipalExtensions.given;

public class a_claims_principal
{
    public static IEnumerable<Claim> dolittle_claims;
    public static IEnumerable<System.Security.Claims.Claim> dotnet_claims;
    public static ClaimsPrincipal instance;

    Establish context = () =>
    {
        dotnet_claims = new List<System.Security.Claims.Claim>
        {
            new(ClaimTypes.Name, "Michael"),
            new(ClaimTypes.Country, "Norway"),
            new(ClaimTypes.Gender, "M"),
            new(ClaimTypes.Surname, "Smith"),
            new(ClaimTypes.Email, "michael@dolittle.com"),
            new(ClaimTypes.Role, "Coffee Maker")
        };

        dolittle_claims = dotnet_claims.Select(Claim.FromDotnetClaim);

        instance = new ClaimsPrincipal(new ClaimsIdentity(dotnet_claims));
    };
}