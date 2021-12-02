// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Runtime.Aggregates.for_AggregateRootInstances.when_getting_all;

public class and_there_are_no_registered_or_stored_aggregates : given.all_dependencies
{
    static IEnumerable<AggregateRootWithInstances> result;

    Establish context = () =>
    {
        setup_aggregate_roots();
        setup_aggregate_root_instances_fetcher();
    };

    Because of = () => result = aggregate_root_instances.GetAll().GetAwaiter().GetResult();

    It should_not_get_any_aggregates = () => result.ShouldBeEmpty();
    It should_not_try_fetch_any_aggregates = () => aggregates_fetcher.VerifyNoOtherCalls();
        
}