// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_creating;

public class with_multiple_definers_for_default_policy
{
    static IEnumerable<IDefineDefaultPolicy> default_policy_definers;
    static Exception result;

    Establish context = () =>
    {
        var firstDefiner = new Mock<IDefineDefaultPolicy>();
        var secondDefiner = new Mock<IDefineDefaultPolicy>();
        default_policy_definers = new StaticInstancesOf<IDefineDefaultPolicy>(
            firstDefiner.Object,
            secondDefiner.Object);
    };

    Because of = () => result = Catch.Exception(() => new Policies(
        default_policy_definers,
        new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
        new StaticInstancesOf<IDefineNamedPolicy>(),
        new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
        new StaticInstancesOf<IDefinePolicyForType>(),
        new StaticInstancesOf<IDefineAsyncPolicyForType>()));

    It should_throw_multiple_default_policy_definers_found = () => result.ShouldBeOfExactType<MultipleDefaultPolicyDefinersFound>();
}