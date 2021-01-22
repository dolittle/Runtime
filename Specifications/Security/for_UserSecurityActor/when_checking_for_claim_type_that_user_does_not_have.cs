// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Dolittle.Runtime.Security;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_UserSecurityActor
{
    public class when_checking_for_claim_type_that_user_does_not_have : given.a_user_security_actor
    {
        static bool result;

        Because of = () => result = actor.HasClaimType("Something");

        It should_return_false = () => result.ShouldBeFalse();
    }
}
