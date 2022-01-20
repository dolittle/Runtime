// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Runtime.Types;
using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Resilience.for_Policies.when_creating;

public class with_multiple_definers_for_default_async_policy
{
    static IInstancesOf<IDefineDefaultAsyncPolicy> default_async_policy_definers;
    static Exception result;

    Establish context = () =>
    {
        var firstDefiner = new Mock<IDefineDefaultAsyncPolicy>();
        var secondDefiner = new Mock<IDefineDefaultAsyncPolicy>();
        default_async_policy_definers = new StaticInstancesOf<IDefineDefaultAsyncPolicy>(
            firstDefiner.Object,
            secondDefiner.Object);
    };

    Because of = () => result = Catch.Exception(() => new Policies(
        new StaticInstancesOf<IDefineDefaultPolicy>(),
        default_async_policy_definers,
        new StaticInstancesOf<IDefineNamedPolicy>(),
        new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
        new StaticInstancesOf<IDefinePolicyForType>(),
        new StaticInstancesOf<IDefineAsyncPolicyForType>()));

    It should_throw_multiple_default_policy_definers_found = () => result.ShouldBeOfExactType<MultipleDefaultPolicyDefinersFound>();
}