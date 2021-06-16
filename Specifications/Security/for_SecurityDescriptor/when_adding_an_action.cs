// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor
{
    public class when_adding_an_action
    {
        static SecurityDescriptor security_descriptor;
        static Mock<ISecurityAction> security_action_mock;

        Establish context = () =>
        {
            security_descriptor = new SecurityDescriptor();
            security_action_mock = new Mock<ISecurityAction>();
        };

        Because of = () => security_descriptor.AddAction(security_action_mock.Object);

        It should_have_it_available_as_action_from_the_enumerable = () => security_descriptor.Actions.ShouldContain(security_action_mock.Object);
    }
}
