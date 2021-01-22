// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_AsyncPolicy.when_executing_action
{
    public class with_result_with_underlying_policy
    {
        const string expected_result = "Fourty Two";
        static AsyncPolicy policy;
        static string result;

        Establish context = () => policy = new AsyncPolicy(Polly.Policy.NoOpAsync());

        Because of = () => result = policy.Execute(() => Task.FromResult(expected_result)).GetAwaiter().GetResult();

        It should_forward_call_to_delegated_policy_and_return_result = () => result.ShouldEqual(expected_result);
    }
}