// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Resilience.for_Policies.when_getting_default.policy;

public class and_no_default_policy_has_been_defined : given.no_default_policy
{
    static IPolicy policy;

    Because of = () => policy = policies.Default;

    It should_return_a_null_policy = () => policy.ShouldBeOfExactType<PassThroughPolicy>();
}