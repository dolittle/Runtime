// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;

namespace Dolittle.Runtime.Aggregates.for_AggregateRootInstances.when_getting_for
{
    public class and_aggregate_root_is_not_registered_but_is_in_event_store : given.all_dependencies
    {
        static AggregateRootWithInstances result;

        Establish context = () =>
        {
            setup_aggregate_roots();
            setup_aggregate_root_instances_fetcher((an_aggregate_root, new []{ an_aggregate_root_instance }));
        };

        Because of = () => result = aggregate_root_instances.GetFor(an_aggregate_root_id).GetAwaiter().GetResult();

        It should_not_get_any_aggregates = () => result.Instances.ShouldBeEmpty();
        It should_not_try_fetch_any_aggregates = () => aggregates_fetcher.VerifyNoOtherCalls();
    }
}
