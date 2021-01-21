// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_AsyncPolicy.when_executing_action
{
    public class with_result_with_delegated_policy
    {
        const string expected_result = "Fourty Two";
        static AsyncPolicy policy;
        static Mock<IAsyncPolicy> delegated_policy;
        static string result;

        Establish context = () =>
        {
            delegated_policy = new Mock<IAsyncPolicy>();
            policy = new AsyncPolicy(delegated_policy.Object);
            delegated_policy.Setup(_ => _.Execute<string>(Moq.It.IsAny<Func<CancellationToken, Task<string>>>(), Moq.It.IsAny<bool>(), Moq.It.IsAny<CancellationToken>())).Returns(() => Task.FromResult(expected_result));
        };

        Because of = () => result = policy.Execute(() => Task.FromResult(expected_result)).GetAwaiter().GetResult();

        It should_forward_call_to_delegated_policy_and_return_result = () => result.ShouldEqual(expected_result);
    }
}