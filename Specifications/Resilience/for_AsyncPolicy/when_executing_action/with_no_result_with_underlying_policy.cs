// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_AsyncPolicy.when_executing_action
{
    public class with_no_result_with_underlying_policy
    {
        static AsyncPolicy policy;
        static bool called;

        Establish context = () => policy = new AsyncPolicy(Polly.Policy.NoOpAsync());

        Because of = () => policy.Execute(() =>
        {
            called = true;
            return Task.CompletedTask;
        }).GetAwaiter().GetResult();

        It should_call_the_underlying_policy = () => called.ShouldBeTrue();
    }
}