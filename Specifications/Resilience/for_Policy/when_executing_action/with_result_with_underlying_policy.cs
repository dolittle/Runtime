// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policy.when_executing_action;

public class with_result_with_underlying_policy
{
    const string expected_result = "Fourty Two";
    static Policy policy;
    static string result;

    Establish context = () => policy = new Policy(Polly.Policy.NoOp());

    Because of = () => result = policy.Execute(() => expected_result);

    It should_forward_call_to_delegated_policy_and_return_result = () => result.ShouldEqual(expected_result);
}