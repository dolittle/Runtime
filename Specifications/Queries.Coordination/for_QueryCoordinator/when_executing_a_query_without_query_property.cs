// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class when_executing_a_query_without_query_property : given.a_query_coordinator
    {
        static IQuery query;
        static PagingInfo paging;
        static Exception exception;

        Establish context = () =>
        {
            query = new QueryWithoutProperty();
            paging = new PagingInfo();
        };

        Because of = () => exception = Catch.Exception(() => coordinator.Execute(query, paging));

        It should_throw_missing_query_property = () => exception.ShouldBeOfExactType<MissingQueryProperty>();
    }
}
