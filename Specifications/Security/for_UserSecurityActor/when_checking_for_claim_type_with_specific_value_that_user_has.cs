// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Dolittle.Runtime.Security;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_UserSecurityActor
{
    public class when_checking_for_claim_type_with_specific_value_that_user_has : given.a_user_security_actor
    {
        const string claim_type = "Something";
        static bool result;

        Establish context = () => identity.AddClaim(new System.Security.Claims.Claim(claim_type, "42"));

        Because of = () => result = actor.HasClaimTypeWithValue(claim_type, "42");

        It should_return_true = () => result.ShouldBeTrue();
    }
}