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
    public class with_no_result_with_delegated_policy
    {
        static AsyncPolicy policy;
        static Mock<IAsyncPolicy> delegated_policy;

        Establish context = () =>
        {
            delegated_policy = new Mock<IAsyncPolicy>();
            policy = new AsyncPolicy(delegated_policy.Object);
        };

        Because of = () => policy.Execute(() => Task.CompletedTask).GetAwaiter().GetResult();

        It should_forward_call_to_delegated_policy = () => delegated_policy.Verify(_ => _.Execute(Moq.It.IsAny<Func<CancellationToken, Task>>(), Moq.It.IsAny<bool>(), Moq.It.IsAny<CancellationToken>()), Times.Once);
    }
}