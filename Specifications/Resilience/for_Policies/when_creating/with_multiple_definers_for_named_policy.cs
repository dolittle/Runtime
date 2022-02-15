// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_creating;

public class with_multiple_definers_for_named_policy
{
    const string policy_name = "Fourty Two";
    static IEnumerable<IDefineNamedPolicy> named_policy_definers;
    static Exception result;

    Establish context = () =>
    {
        var firstDefiner = new Mock<IDefineNamedPolicy>();
        firstDefiner.SetupGet(_ => _.Name).Returns(policy_name);
        var secondDefiner = new Mock<IDefineNamedPolicy>();
        secondDefiner.SetupGet(_ => _.Name).Returns(policy_name);
        named_policy_definers = new StaticInstancesOf<IDefineNamedPolicy>(
            firstDefiner.Object,
            secondDefiner.Object);
    };

    Because of = () => result = Catch.Exception(() => new Policies(
        new StaticInstancesOf<IDefineDefaultPolicy>(),
        new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
        named_policy_definers,
        new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
        new StaticInstancesOf<IDefinePolicyForType>(),
        new StaticInstancesOf<IDefineAsyncPolicyForType>()));

    It should_throw_multiple_policy_definers_for_name_found = () => result.ShouldBeOfExactType<MultiplePolicyDefinersForNameFound>();
}