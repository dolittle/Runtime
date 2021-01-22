// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policy.when_executing_action
{
    public class with_result_with_delegated_policy
    {
        const string expected_result = "Fourty Two";
        static Policy policy;
        static Mock<IPolicy> delegated_policy;
        static string result;

        Establish context = () =>
        {
            delegated_policy = new Mock<IPolicy>();
            policy = new Policy(delegated_policy.Object);
            delegated_policy.Setup(_ => _.Execute(Moq.It.IsAny<Func<string>>())).Returns(() => expected_result);
        };

        Because of = () => result = policy.Execute(() => expected_result);

        It should_forward_call_to_delegated_policy_and_return_result = () => result.ShouldEqual(expected_result);
    }
}