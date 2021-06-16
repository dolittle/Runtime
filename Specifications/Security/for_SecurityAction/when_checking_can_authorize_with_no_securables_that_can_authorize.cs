// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityAction
{
    [Subject(typeof(SecurityAction))]
    public class when_checking_can_authorize_with_no_securables_that_can_authorize
    {
        static Mock<ISecurityTarget> target_that_cannot_authorize;
        static Mock<ISecurityTarget> another_target_that_cannot_authorize;
        static SecurityAction action;
        static bool can_authorize;

        Establish context = () =>
        {
            target_that_cannot_authorize = new Mock<ISecurityTarget>();
            target_that_cannot_authorize.Setup(s => s.CanAuthorize(Moq.It.IsAny<object>())).Returns(false);
            another_target_that_cannot_authorize = new Mock<ISecurityTarget>();
            another_target_that_cannot_authorize.Setup(s => s.CanAuthorize(Moq.It.IsAny<object>())).Returns(false);
            action = new SecurityAction();
            action.AddTarget(another_target_that_cannot_authorize.Object);
            action.AddTarget(target_that_cannot_authorize.Object);
        };

        Because of = () => can_authorize = action.CanAuthorize(new object());

        It should_be_authorizable = () => can_authorize.ShouldBeFalse();
    }
}