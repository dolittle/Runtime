// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Aggregates.for_Aggregates.when_getting_for
{
    public class and_aggregate_root_is_registered_and_is_in_event_store : given.all_dependencies
    {
        static IEnumerable<AggregateRootInstance> result;
        
        Establish context = () =>
        {
            setup_aggregate_roots(an_aggregate_root);
            setup_aggregate_root_instances_fetcher((an_aggregate_root, new []{ an_aggregate_root_instance } ));
        };

        Because of = () => result = aggregate_root_instances.GetFor(an_aggregate_root).GetAwaiter().GetResult();

        It should_get_the_aggregates = () => result.ShouldContainOnly(an_aggregate_root_instance);

        It should_fetch_the_aggregates_for_the_correct_root = () => aggregates_fetcher.Verify(_ => _.FetchFor(an_aggregate_root), Times.Once);
    }
}
