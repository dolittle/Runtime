// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_default.async_policy
{
    public class and_it_is_defined : given.defined_default_policy
    {
        static IPolicy policy;
        Because of = () => policy = policies.Default;

        It should_return_a_policy = () => policy.ShouldBeOfExactType<Policy>();
        It should_pass_the_underlying_policy = () => ((Policy)policy).UnderlyingPolicy.ShouldEqual(underlying_sync_policy);
    }
}