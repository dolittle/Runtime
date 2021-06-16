// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policy.when_executing_action
{
    public class with_no_result_with_delegated_policy
    {
        static Policy policy;
        static Mock<IPolicy> delegated_policy;

        Establish context = () =>
        {
            delegated_policy = new Mock<IPolicy>();
            policy = new Policy(delegated_policy.Object);
        };

        Because of = () => policy.Execute(() => { });

        It should_forward_call_to_delegated_policy = () => delegated_policy.Verify(_ => _.Execute(Moq.It.IsAny<Action>()), Times.Once);
    }
}