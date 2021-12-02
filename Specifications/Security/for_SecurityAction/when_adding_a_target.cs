// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityAction;

public class when_adding_a_target
{
    static SecurityAction security_action;
    static Mock<ISecurityTarget> security_target_mock;

    Establish context = () =>
    {
        security_action = new SecurityAction();
        security_target_mock = new Mock<ISecurityTarget>();
    };

    Because of = () => security_action.AddTarget(security_target_mock.Object);

    It should_have_it_available_in_the_collection = () => security_action.Targets.ShouldContain(security_target_mock.Object);
}