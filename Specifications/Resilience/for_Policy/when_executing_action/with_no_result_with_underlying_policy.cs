// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policy.when_executing_action
{
    public class with_no_result_with_underlying_policy
    {
        static Policy policy;
        static bool called;

        Establish context = () => policy = new Policy(Polly.Policy.NoOp());

        Because of = () => policy.Execute(() => called = true);

        It should_call_the_underlying_policy = () => called.ShouldBeTrue();
    }
}