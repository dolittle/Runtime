// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_UserSecurityActor;

public class when_checking_for_claim_type_that_user_has_without_correct_value : given.a_user_security_actor
{
    const string claim_type = "Something";
    static bool result;

    Establish context = () => identity.AddClaim(new System.Security.Claims.Claim(claim_type, "42"));

    Because of = () => result = actor.HasClaimTypeWithValue("Something", "43");

    It should_not_have_the_claim = () => result.ShouldBeFalse();
}