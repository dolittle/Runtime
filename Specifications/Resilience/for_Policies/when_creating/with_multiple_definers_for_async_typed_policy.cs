// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_creating;

public class with_multiple_definers_for_async_typed_policy
{
    static Type policy_type = typeof(string);
    static IEnumerable<IDefineAsyncPolicyForType> typed_async_policy_definers;
    static Exception result;

    Establish context = () =>
    {
        var firstDefiner = new Mock<IDefineAsyncPolicyForType>();
        firstDefiner.SetupGet(_ => _.Type).Returns(policy_type);
        var secondDefiner = new Mock<IDefineAsyncPolicyForType>();
        secondDefiner.SetupGet(_ => _.Type).Returns(policy_type);
        typed_async_policy_definers = new StaticInstancesOf<IDefineAsyncPolicyForType>(
            firstDefiner.Object,
            secondDefiner.Object);
    };

    Because of = () => result = Catch.Exception(() => new Policies(
        new StaticInstancesOf<IDefineDefaultPolicy>(),
        new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
        new StaticInstancesOf<IDefineNamedPolicy>(),
        new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
        new StaticInstancesOf<IDefinePolicyForType>(),
        typed_async_policy_definers));

    It should_throw_multiple_policy_definers_for_type_found = () => result.ShouldBeOfExactType<MultiplePolicyDefinersForTypeFound>();
}