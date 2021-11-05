// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Dolittle.Runtime.Events.Store;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Aggregates.for_AggregateRootInstances.when_getting_for
{
    public class and_aggregate_root_is_registered_but_is_not_in_event_store : given.all_dependencies
    {
        static AggregateRootWithInstances result;

        Establish context = () =>
        {
            setup_aggregate_roots(an_aggregate_root);
            setup_aggregate_root_instances_fetcher();
        };

        Because of = () => result = aggregate_root_instances.GetFor(an_aggregate_root_id).GetAwaiter().GetResult();

        It should_not_get_any_aggregates = () => result.Instances.ShouldBeEmpty();
        It should_fetch_the_aggregates_for_the_correct_root = () => aggregates_fetcher.Verify(_ => _.FetchFor(an_aggregate_root_id), Times.Once);
    }
}
