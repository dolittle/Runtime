// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policy.when_creating;

public class with_delegated_policy
{
    static IPolicy delegated_policy;
    static Policy policy;

    Establish context = () => delegated_policy = new Policy(Polly.Policy.NoOp());

    Because of = () => policy = new Policy(delegated_policy);

    It should_not_have_underlying_policy = () => policy.UnderlyingPolicy.ShouldBeNull();
    It should_have_delegated_policy = () => policy.DelegatedPolicy.ShouldNotBeNull();
    It should_have_the_correct_underlying_policy = () => policy.DelegatedPolicy.ShouldEqual(delegated_policy);
}