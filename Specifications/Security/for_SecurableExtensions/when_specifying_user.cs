// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurableExtensions
{
    public class when_specifying_user
    {
        static TypeSecurable securable;
        static ISecurityActor actor;
        static ICanResolvePrincipal principalResolver;

        Establish context = () =>
        {
            securable = new TypeSecurable(typeof(object));
            principalResolver = new Mock<ICanResolvePrincipal>().Object;
        };

        Because of = () => actor = securable.UserFrom(principalResolver);

        It should_return_an_user_actor_builder = () => actor.ShouldBeOfExactType<UserSecurityActor>();
        It should_add_actor_to_securable = () => securable.Actors.First().ShouldBeOfExactType<UserSecurityActor>();
    }
}
