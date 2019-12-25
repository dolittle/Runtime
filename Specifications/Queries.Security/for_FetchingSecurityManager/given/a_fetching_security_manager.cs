// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Security;
using Machine.Specifications;
using Moq;

namespace Dolittle.Queries.Security.Specs.for_FetchingSecurityManager.given
{
    public class a_fetching_security_manager
    {
        protected static Mock<ISecurityManager> security_manager_mock;
        protected static FetchingSecurityManager fetching_security_manager;

        Establish context = () =>
        {
            security_manager_mock = new Mock<ISecurityManager>();
            fetching_security_manager = new FetchingSecurityManager(security_manager_mock.Object);
        };
    }
}
