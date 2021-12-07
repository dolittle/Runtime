// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Types.Testing;
using Machine.Specifications;

namespace Dolittle.Runtime.Resilience.for_Policies.given;

public class no_default_policy
{
    protected static Policies policies;

    Establish context = () =>
    {
        policies = new Policies(
            new StaticInstancesOf<IDefineDefaultPolicy>(),
            new StaticInstancesOf<IDefineDefaultAsyncPolicy>(),
            new StaticInstancesOf<IDefineNamedPolicy>(),
            new StaticInstancesOf<IDefineNamedAsyncPolicy>(),
            new StaticInstancesOf<IDefinePolicyForType>(),
            new StaticInstancesOf<IDefineAsyncPolicyForType>());
    };
}