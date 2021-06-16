// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Moq;

namespace Dolittle.Runtime.Security.Specs.for_RoleRule.given
{
    public class a_rule_role
    {
        protected static string required_role;
        protected static Mock<IUserSecurityActor> user;
        protected static RoleRule rule;

        public a_rule_role()
        {
            required_role = "MY_ROLE";
            user = new Mock<IUserSecurityActor>();
            user.Setup(m => m.IsInRole(It.IsAny<string>())).Returns(false);
            rule = new RoleRule(user.Object, required_role);
        }

        protected static void SetUserRole(string role)
        {
            SetUserRoles(new[] { role });
        }

        protected static void SetUserRoles(string[] roles)
        {
            user.Setup(m => m.IsInRole(It.IsAny<string>()))
                .Returns((string r) => roles.Any(s => s == r));
        }
    }
}