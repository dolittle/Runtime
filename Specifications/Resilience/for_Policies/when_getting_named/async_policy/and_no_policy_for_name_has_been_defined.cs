// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_named.async_policy
{
    public class and_no_policy_for_name_has_been_defined : given.defined_default_policy
    {
        static NamedAsyncPolicy policy;

        Because of = () => policy = policies.GetAsyncNamed("FourtyTwo") as NamedAsyncPolicy;

        It should_return_policy_that_delegates_to_default_policy = () => policy.DelegatedPolicy.ShouldEqual(policies.DefaultAsync);
    }
}