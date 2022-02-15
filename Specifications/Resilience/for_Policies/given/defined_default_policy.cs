// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using Moq;

namespace Dolittle.Runtime.Resilience.for_Policies.given;

public class defined_default_policy
{
    protected static Policies policies;
    protected static Mock<IDefineDefaultPolicy> sync_policy_definer;
    protected static Mock<IDefineDefaultAsyncPolicy> async_policy_definer;
    protected static Polly.ISyncPolicy underlying_sync_policy;
    protected static Polly.IAsyncPolicy underlying_async_policy;

    Establish context = () =>
    {
        sync_policy_definer = new Mock<IDefineDefaultPolicy>();
        async_policy_definer = new Mock<IDefineDefaultAsyncPolicy>();
        underlying_sync_policy = Polly.Policy.NoOp();
        underlying_async_policy = Polly.Policy.NoOpAsync();
        sync_policy_definer.Setup(_ => _.Define()).Returns(underlying_sync_policy);
        async_policy_definer.Setup(_ => _.Define()).Returns(underlying_async_policy);
        policies = new Policies(
            new StaticInstancesOf<IDefineDefaultPolicy>(sync_policy_definer.Object),
            new StaticInstancesOf<IDefineDefaultAsyncPolicy>(async_policy_definer.Object),
            new StaticInstancesOf<IDefineNamedPolicy>(),
            new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
            new StaticInstancesOf<IDefinePolicyForType>(),
            new StaticInstancesOf<IDefineAsyncPolicyForType>());
    };
}