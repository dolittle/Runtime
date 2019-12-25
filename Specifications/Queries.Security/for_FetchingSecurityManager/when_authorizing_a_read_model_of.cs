// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.ReadModels;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Queries.Security.Specs.for_FetchingSecurityManager
{
    public class when_authorizing_a_read_model_of : given.a_fetching_security_manager
    {
        static IReadModelOf<SomeReadModel> read_model_of;

        Establish context = () => read_model_of = new Mock<IReadModelOf<SomeReadModel>>().Object;

        Because of = () => fetching_security_manager.Authorize(read_model_of);

        It should_delegate_the_request_for_security_to_the_security_manager = () => security_manager_mock.Verify(s => s.Authorize<Fetching>(read_model_of), Moq.Times.Once());
    }
}
