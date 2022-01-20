// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_AuthorizationDescriptorResult;

[Subject(typeof(AuthorizeDescriptorResult))]
public class when_building_the_failed_authorization_messages
{
    static Mock<AuthorizeActionResult> first_failed_authorization;
    static Mock<AuthorizeActionResult> second_failed_authorization;
    static string first_action_first_description = "first_action_first_description";
    static string first_action_second_description = "first_action_second_description";
    static string second_action_description = "second_action";
    static AuthorizeDescriptorResult result;
    static IEnumerable<string> failed_authorization_messages;

    Establish context = () =>
    {
        first_failed_authorization = new Mock<AuthorizeActionResult>(new Mock<ISecurityAction>().Object);
        first_failed_authorization.Setup(a => a.BuildFailedAuthorizationMessages())
            .Returns(new[] { first_action_first_description, first_action_second_description });
        second_failed_authorization = new Mock<AuthorizeActionResult>(new Mock<ISecurityAction>().Object);
        second_failed_authorization.Setup(a => a.BuildFailedAuthorizationMessages())
            .Returns(new[] { second_action_description });

        result = new AuthorizeDescriptorResult();
        result.ProcessAuthorizeActionResult(first_failed_authorization.Object);
        result.ProcessAuthorizeActionResult(second_failed_authorization.Object);
    };

    Because of = () => failed_authorization_messages = result.BuildFailedAuthorizationMessages();

    It have_three_failed_authorization_messages = () => failed_authorization_messages.Count().ShouldEqual(3);
    It should_have_one_message_with_the_first_action_first_message = () => failed_authorization_messages.Count(m => m.EndsWith(first_action_first_description)).ShouldEqual(1);
    It should_have_one_message_with_the_first_action_second_message = () => failed_authorization_messages.Count(m => m.EndsWith(first_action_second_description)).ShouldEqual(1);
    It should_have_one_message_with_the_second_action_message = () => failed_authorization_messages.Count(m => m.EndsWith(second_action_description)).ShouldEqual(1);
}