// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.AggregateRoots.for_AggregateRoots.when_registering
{
    public class and_there_are_no_other_registrations : given.all_dependencies
    {
        Because of = () => aggregate_roots.Register(an_aggregate_root);

        It should_only_have_the_registered_aggregate_root = () => aggregate_roots.All.ShouldContainOnly(an_aggregate_root);
    }
}
