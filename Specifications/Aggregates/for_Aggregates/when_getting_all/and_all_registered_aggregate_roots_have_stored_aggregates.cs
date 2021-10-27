// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Aggregates.for_Aggregates.when_getting_all
{
    public class and_all_registered_aggregate_roots_have_stored_aggregates : given.all_dependencies
    {
        static IEnumerable<AggregateRootWithInstances> result;

        Establish context = () =>
        {
            setup_aggregate_roots(an_aggregate_root);
            setup_aggregate_root_instances_fetcher((an_aggregate_root, new []{ an_aggregate_root_instance }));
        };

        Because of = () => result = aggregate_root_instances.GetAll().GetAwaiter().GetResult();

        It should_get_the_correct_aggregate_roots = () => result.Select(_ => _.Root).ShouldContainOnly(an_aggregate_root);

        It should_have_the_correct_aggregates = () => result.ToArray()[0].Instances.ShouldContainOnly(new []{ an_aggregate_root_instance });

        It should_fetch_the_aggregates_for_the_correct_root = () => aggregates_fetcher.Verify(_ => _.FetchFor(an_aggregate_root), Times.Once);
    }
}
