// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityTarget;

public class when_adding_a_securable
{
    static SecurityTarget security_target;
    static Mock<ISecurable> securable_mock;

    Establish context = () =>
    {
        security_target = new SecurityTarget(string.Empty);
        securable_mock = new Mock<ISecurable>();
    };

    Because of = () => security_target.AddSecurable(securable_mock.Object);

    It should_have_it_available_in_the_collection = () => security_target.Securables.ShouldContain(securable_mock.Object);
}