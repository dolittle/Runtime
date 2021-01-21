// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Security.for_ClaimsPrincipalExtensions.when_converting_to_claims
{
    [Subject(typeof(ClaimsPrincipalExtensions), nameof(ClaimsPrincipalExtensions.ToClaims))]
    public class from_a_simple_claims_principal : given.a_claims_principal
    {
        static Claims result;

        Because of = () => result = instance.ToClaims();

        It should_create_an_instance_of_claims = () => result.ShouldBeOfExactType<Claims>();
        It should_have_all_the_claims = () => result.ShouldContainOnly(dolittle_claims);
    }
}