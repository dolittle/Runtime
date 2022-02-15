// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_named.policy;

public class and_policy_for_name_is_defined
{
    const string name = "Fourty Two";
    static NamedPolicy named_policy;

    static Polly.Policy underlying_policy;

    static Policies policies;

    static Mock<IDefineNamedPolicy> named_policy_definer;

    Establish context = () =>
    {
        underlying_policy = Polly.Policy.NoOp();
        named_policy_definer = new Mock<IDefineNamedPolicy>();
        named_policy_definer.SetupGet(_ => _.Name).Returns(name);
        named_policy_definer.Setup(_ => _.Define()).Returns(underlying_policy);

        policies = new Policies(
            new StaticInstancesOf<IDefineDefaultPolicy>(),
            new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
            new StaticInstancesOf<IDefineNamedPolicy>(named_policy_definer.Object),
            new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
            new StaticInstancesOf<IDefinePolicyForType>(),
            new StaticInstancesOf<IDefineAsyncPolicyForType>());
    };

    Because of = () => named_policy = policies.GetNamed(name) as NamedPolicy;

    It should_return_a_policy = () => named_policy.ShouldNotBeNull();
    It should_pass_the_underlying_policy = () => named_policy.UnderlyingPolicy.ShouldEqual(underlying_policy);
}