// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_typed.async_policy
{
    public class and_policy_for_type_is_defined
    {
        static Type type = typeof(string);
        static AsyncPolicyFor<string> typed_policy;
        static Polly.AsyncPolicy underlying_policy;

        static Policies policies;

        static Mock<IDefineAsyncPolicyForType> typed_policy_definer;

        Establish context = () =>
        {
            underlying_policy = Polly.Policy.NoOpAsync();
            typed_policy_definer = new Mock<IDefineAsyncPolicyForType>();
            typed_policy_definer.SetupGet(_ => _.Type).Returns(type);
            typed_policy_definer.Setup(_ => _.Define()).Returns(underlying_policy);

            policies = new Policies(
                new StaticInstancesOf<IDefineDefaultPolicy>(),
                new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
                new StaticInstancesOf<IDefineNamedPolicy>(),
                new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
                new StaticInstancesOf<IDefinePolicyForType>(),
                new StaticInstancesOf<IDefineAsyncPolicyForType>(typed_policy_definer.Object));
        };

        Because of = () => typed_policy = policies.GetAsyncFor<string>() as AsyncPolicyFor<string>;

        It should_return_a_policy = () => typed_policy.ShouldNotBeNull();
        It should_pass_the_underlying_policy = () => typed_policy.UnderlyingPolicy.ShouldEqual(underlying_policy);
    }
}