// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Security.Specs.for_ClaimTypeRule.given
{
    public class a_claim_type_rule
    {
        protected const string required_claim = "MY CLAIM";
        protected static Mock<IUserSecurityActor> user;
        protected static ClaimTypeRule rule;

        Establish context = () =>
        {
            user = new Mock<IUserSecurityActor>();
            rule = new ClaimTypeRule(user.Object, required_claim);
        };
    }
}