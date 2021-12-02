// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_named.async_policy;

public class and_policy_for_name_is_defined
{
    const string name = "Fourty Two";
    static NamedAsyncPolicy named_policy;

    static Polly.AsyncPolicy underlying_policy;

    static Policies policies;

    static Mock<IDefineNamedAsyncPolicy> named_policy_definer;

    Establish context = () =>
    {
        underlying_policy = Polly.Policy.NoOpAsync();
        named_policy_definer = new Mock<IDefineNamedAsyncPolicy>();
        named_policy_definer.SetupGet(_ => _.Name).Returns(name);
        named_policy_definer.Setup(_ => _.Define()).Returns(underlying_policy);

        policies = new Policies(
            new StaticInstancesOf<IDefineDefaultPolicy>(),
            new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
            new StaticInstancesOf<IDefineNamedPolicy>(),
            new StaticInstancesOf<IDefineNamedAsyncPolicy>(named_policy_definer.Object),
            new StaticInstancesOf<IDefinePolicyForType>(),
            new StaticInstancesOf<IDefineAsyncPolicyForType>());
    };

    Because of = () => named_policy = policies.GetAsyncNamed(name) as NamedAsyncPolicy;

    It should_return_a_policy = () => named_policy.ShouldNotBeNull();
    It should_pass_the_underlying_policy = () => named_policy.UnderlyingPolicy.ShouldEqual(underlying_policy);
}