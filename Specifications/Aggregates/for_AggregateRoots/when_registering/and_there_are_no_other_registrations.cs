// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRoots.when_registering
{
    public class and_there_are_no_other_registrations : given.all_dependencies
    {
        Because of = () => aggregate_roots.Register(AnAggregateRoot);

        It should_only_have_the_registered_aggregate_root = () => aggregate_roots.All.ShouldContainOnly(AnAggregateRoot);
    }
}
