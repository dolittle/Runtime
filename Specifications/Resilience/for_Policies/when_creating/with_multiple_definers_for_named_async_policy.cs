// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_creating;

public class with_multiple_definers_for_named_async_policy
{
    const string policy_name = "Fourty Two";
    static IEnumerable<IDefineNamedAsyncPolicy> named_async_policy_definers;
    static Exception result;

    Establish context = () =>
    {
        var firstDefiner = new Mock<IDefineNamedAsyncPolicy>();
        firstDefiner.SetupGet(_ => _.Name).Returns(policy_name);
        var secondDefiner = new Mock<IDefineNamedAsyncPolicy>();
        secondDefiner.SetupGet(_ => _.Name).Returns(policy_name);
        named_async_policy_definers = new StaticInstancesOf<IDefineNamedAsyncPolicy>(
            firstDefiner.Object,
            secondDefiner.Object);
    };

    Because of = () => result = Catch.Exception(() => new Policies(
        new StaticInstancesOf<IDefineDefaultPolicy>(),
        new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
        new StaticInstancesOf<IDefineNamedPolicy>(),
        named_async_policy_definers,
        new StaticInstancesOf<IDefinePolicyForType>(),
        new StaticInstancesOf<IDefineAsyncPolicyForType>()));

    It should_throw_multiple_policy_definers_for_name_found = () => result.ShouldBeOfExactType<MultiplePolicyDefinersForNameFound>();
}