// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Security.Specs.for_SecurityManager
{
    [Subject(typeof(SecurityManager))]
    public class when_checking_is_authorized_without_any_security_descriptors : given.a_security_manager_with_no_descriptors
    {
        const string securable = "something";

        static AuthorizationResult result;

        Because of = () => result = security_manager.Authorize<SomeSecurityAction>(securable);

        It should_return_true = () => result.IsAuthorized.ShouldBeTrue();
    }
}
