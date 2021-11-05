// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRoots.when_registering
{
    public class an_aggregate_root_that_is_already_registered : given.all_dependencies
    {
        Establish context = () => aggregate_roots.Register(an_aggregate_root with { Alias = "Some other alias" });
        Because of = () => aggregate_roots.Register(an_aggregate_root);

        It should_only_have_the_last_registered_aggregate_root = () => aggregate_roots.All.ShouldContainOnly(an_aggregate_root);
    }
}
