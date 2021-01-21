// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Security.Claims;
using Dolittle.Runtime.Security;
using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Security.Specs.for_UserSecurityActor.given
{
    public class a_user_security_actor
    {
        protected static UserSecurityActor actor;
        protected static ClaimsIdentity identity;
        protected static ClaimsPrincipal principal;

        Establish context = () =>
        {
            identity = new ClaimsIdentity();
            principal = new ClaimsPrincipal(identity);

            var principalResolver = new Mock<ICanResolvePrincipal>();
            principalResolver.Setup(p => p.Resolve()).Returns(principal);

            actor = new UserSecurityActor(principalResolver.Object);
        };
    }
}