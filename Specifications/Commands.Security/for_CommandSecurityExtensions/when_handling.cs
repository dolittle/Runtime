// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Commands.Security.Specs.for_CommandSecurityExtensions
{
    public class when_handling
    {
        static Mock<ISecurityDescriptorBuilder> security_descriptor_builder_mock;
        static Mock<ISecurityDescriptor> security_descriptor_mock;

        Establish context = () =>
        {
            security_descriptor_builder_mock = new Mock<ISecurityDescriptorBuilder>();
            security_descriptor_mock = new Mock<ISecurityDescriptor>();
            security_descriptor_builder_mock.SetupGet(s => s.Descriptor).Returns(security_descriptor_mock.Object);
        };

        Because of = () => security_descriptor_builder_mock.Object.Handling();

        It should_add_the_handle_command_action = () => security_descriptor_mock.Verify(s => s.AddAction(Moq.It.IsAny<HandleCommand>()), Times.Once());
    }
}
