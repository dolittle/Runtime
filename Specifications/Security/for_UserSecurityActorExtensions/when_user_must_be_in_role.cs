// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_UserSecurityActorExtensions
{
    public class when_user_must_be_in_role
    {
        static UserSecurityActor actor;

        Establish context = () => actor = new UserSecurityActor(Mock.Of<ICanResolvePrincipal>());

        Because of = () => actor.MustBeInRole("Something");

        It should_add_a_role_rule_to_the_actor = () => actor.Rules.First().ShouldBeOfExactType<RoleRule>();
        It should_pass_required_role_to_the_rule = () => ((RoleRule)actor.Rules.First()).Role.ShouldEqual("Something");
    }
}
