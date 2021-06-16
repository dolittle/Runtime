// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityDescriptor
{
    [Subject(typeof(SecurityDescriptor))]
    public class when_checking_can_authorize_with_target_that_can_authorize
    {
        static ISecurityAction action_that_can_authorize;
        static ISecurityAction action_that_cannot_authorize;
        static SecurityDescriptor descriptor;
        static bool can_authorize;

        public when_checking_can_authorize_with_target_that_can_authorize()
        {
            action_that_can_authorize = new MySecurityAction(_ => true, _ => null);
            action_that_cannot_authorize = new MySecurityAction(_ => true, _ => null);

            descriptor = new SecurityDescriptor();
            descriptor.AddAction(action_that_cannot_authorize);
            descriptor.AddAction(action_that_can_authorize);
        }

        Because of = () => can_authorize = descriptor.CanAuthorize<MySecurityAction>(new object());

        It should_be_authorizable = () => can_authorize.ShouldBeTrue();
    }
}