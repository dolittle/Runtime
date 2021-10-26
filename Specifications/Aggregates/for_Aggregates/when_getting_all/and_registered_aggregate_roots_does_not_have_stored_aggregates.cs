// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Dolittle.Runtime.Aggregates.AggregateRoots;
using Machine.Specifications;
using Moq;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Aggregates.for_Aggregates.when_getting_all
{
    public class and_registered_aggregate_roots_does_not_have_stored_aggregates : given.all_dependencies
    {
        static IEnumerable<(AggregateRoot, IEnumerable<Aggregate>)> result;

        Establish context = () =>
        {
            setup_aggregate_roots(an_aggregate_root);
            setup_aggregates_fetcher();
        };

        Because of = () => result = aggregates.GetAll().GetAwaiter().GetResult();

        It should_get_the_correct_aggregate_roots = () => result.Select(_ => _.Item1).ShouldContainOnly(an_aggregate_root);
        It should_not_have_any_aggregates = () => result.SelectMany(_ => _.Item2).ShouldBeEmpty();
        
        It should_fetch_the_aggregates_for_the_correct_root = () => aggregates_fetcher.Verify(_ => _.FetchFor(an_aggregate_root), Times.Once);
    }
}
