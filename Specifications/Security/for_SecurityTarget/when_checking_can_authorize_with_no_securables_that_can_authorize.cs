﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityTarget;

[Subject(typeof(SecurityTarget))]
public class when_checking_can_authorize_with_no_securables_that_can_authorize
{
    static Mock<ISecurable> securable_that_cannot_authorize;
    static Mock<ISecurable> another_securable_that_cannot_authorize;
    static SecurityTarget target;
    static bool can_authorize;

    Establish context = () =>
    {
        securable_that_cannot_authorize = new Mock<ISecurable>();
        securable_that_cannot_authorize.Setup(s => s.CanAuthorize(Moq.It.IsAny<object>())).Returns(false);
        another_securable_that_cannot_authorize = new Mock<ISecurable>();
        another_securable_that_cannot_authorize.Setup(s => s.CanAuthorize(Moq.It.IsAny<object>())).Returns(false);
        target = new SecurityTarget(string.Empty);
        target.AddSecurable(securable_that_cannot_authorize.Object);
        target.AddSecurable(another_securable_that_cannot_authorize.Object);
    };

    Because of = () => can_authorize = target.CanAuthorize(new object());

    It should_not_be_authorizable = () => can_authorize.ShouldBeFalse();
}