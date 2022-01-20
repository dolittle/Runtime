﻿// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor;

[Subject(typeof(SecurityDescriptor))]
public class when_authorizing_with_an_action_that_is_not_authorized
{
    static SecurityDescriptor descriptor;
    static Mock<ISecurityAction> action_that_authorizes;
    static Mock<ISecurityAction> action_that_does_not_authorize;
    static Mock<ISecurityAction> action_that_cannot_authorize;
    static AuthorizeActionResult authorized_target;
    static AuthorizeActionResult unauthorized_target;
    static AuthorizeDescriptorResult result;

    Establish context = () =>
    {
        var unauthorized_actor_result = new AuthorizeActorResult(null);
        unauthorized_actor_result.AddBrokenRule(new Mock<ISecurityRule>().Object);
        var unauthorized_securable_result = new AuthorizeSecurableResult(null);
        unauthorized_securable_result.ProcessAuthorizeActorResult(unauthorized_actor_result);
        var unauthorized_target_result = new AuthorizeTargetResult(null);
        unauthorized_target_result.ProcessAuthorizeSecurableResult(unauthorized_securable_result);

        descriptor = new SecurityDescriptor();
        action_that_authorizes = new Mock<ISecurityAction>();
        action_that_does_not_authorize = new Mock<ISecurityAction>();
        action_that_cannot_authorize = new Mock<ISecurityAction>();
        authorized_target = new AuthorizeActionResult(action_that_authorizes.Object);
        unauthorized_target = new AuthorizeActionResult(action_that_does_not_authorize.Object);
        unauthorized_target.ProcessAuthorizeTargetResult(unauthorized_target_result);

        action_that_authorizes.Setup(t => t.Authorize(Moq.It.IsAny<object>())).Returns(authorized_target);
        action_that_authorizes.Setup(t => t.CanAuthorize(Moq.It.IsAny<object>())).Returns(true);
        action_that_does_not_authorize.Setup(t => t.Authorize(Moq.It.IsAny<object>())).Returns(unauthorized_target);
        action_that_does_not_authorize.Setup(t => t.CanAuthorize(Moq.It.IsAny<object>())).Returns(true);
        action_that_cannot_authorize.Setup(t => t.CanAuthorize(Moq.It.IsAny<object>())).Returns(false);

        descriptor.AddAction(action_that_authorizes.Object);
        descriptor.AddAction(action_that_does_not_authorize.Object);
        descriptor.AddAction(action_that_cannot_authorize.Object);
    };

    Because of = () => result = descriptor.Authorize(new object());

    It should_not_be_authorized = () => result.IsAuthorized.ShouldBeFalse();

    It should_hold_the_results_of_each_failed_action_authorization = () =>
    {
        result.AuthorizationFailures.Count().ShouldEqual(1);
        result.AuthorizationFailures.All(r => r == unauthorized_target);
    };

    It should_not_attempt_to_authorize_action_that_cannot_authorize = () => action_that_cannot_authorize.Verify(a => a.Authorize(Moq.It.IsAny<object>()), Times.Never());
}