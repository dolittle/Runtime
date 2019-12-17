// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class when_executing_and_provider_throws_an_exception : given.a_query_coordinator_with_known_provider
    {
        static QueryForKnownProvider query;
        static PagingInfo paging;
        static QueryType actual_query;
        static Exception exception_thrown;
        static QueryResult result;

        Establish context = () =>
        {
            query = new QueryForKnownProvider();
            paging = new PagingInfo();

            actual_query = new QueryType();
            query.QueryToReturn = actual_query;

            exception_thrown = new ArgumentException(string.Empty);

            query_provider_mock.Setup(q => q.Execute(actual_query, paging)).Throws(exception_thrown);
        };

        Because of = async () => result = await coordinator.Execute(query, paging).ConfigureAwait(false);

        It should_set_the_exception_on_the_result = () => result.Exception.ShouldEqual(exception_thrown);
    }
}
