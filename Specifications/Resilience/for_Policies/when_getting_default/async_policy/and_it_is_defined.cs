// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_default.policy
{
    public class and_it_is_defined : given.defined_default_policy
    {
        static IAsyncPolicy policy;
        Because of = () => policy = policies.DefaultAsync;

        It should_return_a_policy = () => policy.ShouldBeOfExactType<AsyncPolicy>();
        It should_pass_the_underlying_policy = () => (policy as AsyncPolicy).UnderlyingPolicy.ShouldEqual(underlying_async_policy);
    }
}