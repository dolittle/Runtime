// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Security.Claims;
using Dolittle.Runtime.Collections;
using Machine.Specifications;

namespace Dolittle.Runtime.Security.for_ClaimsPrincipalExtensions.when_converting_to_claims;

[Subject(typeof(ClaimsPrincipalExtensions), nameof(ClaimsPrincipalExtensions.ToClaims))]
public class from_claims : given.a_claims_principal
{
    static Claims claims;
    static ClaimsPrincipal result;

    Establish context = () => claims = instance.ToClaims();
    Because of = () => result = claims.ToClaimsPrincipal();

    It should_create_an_instance_of_claim_principal = () => result.ShouldBeOfExactType<ClaimsPrincipal>();

    It should_have_all_the_claims = () =>
    {
        result.Claims.Count().ShouldEqual(dotnet_claims.Count());
        result.Claims.ForEach(c => dotnet_claims.Any(clm => c.Type == clm.Type && c.ValueType == clm.ValueType && c.Value == clm.Value).ShouldBeTrue());
    };

    It should_have_the_correct_identity = () => result.Identity.Name.ShouldEqual(instance.Identity.Name);
}