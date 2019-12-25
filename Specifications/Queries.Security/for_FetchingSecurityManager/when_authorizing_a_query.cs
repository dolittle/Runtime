// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Queries.Security.Specs.for_FetchingSecurityManager
{
    public class when_authorizing_a_query : given.a_fetching_security_manager
    {
        static SomeQueryFor query_for;

        Establish context = () => query_for = new SomeQueryFor();

        Because of = () => fetching_security_manager.Authorize(query_for);

        It should_delegate_the_request_for_security_to_the_security_manager = () => security_manager_mock.Verify(s => s.Authorize<Fetching>(query_for), Moq.Times.Once());
    }
}
