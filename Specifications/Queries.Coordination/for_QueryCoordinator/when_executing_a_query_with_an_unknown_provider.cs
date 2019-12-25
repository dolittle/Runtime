// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Machine.Specifications;
using It = Machine.Specifications.It;

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class when_executing_a_query_with_an_unknown_provider : given.a_query_coordinator
    {
        static QueryForUnknownProvider query;
        static PagingInfo paging;
        static QueryResult result;

        Establish context = () =>
        {
            query = new QueryForUnknownProvider
            {
                Query = "something"
            };
            paging = new PagingInfo();
        };

        Because of = async () => result = await coordinator.Execute(query, paging).ConfigureAwait(false);

        It should_throw_missing_query_provider = () => result.Exception.ShouldBeOfExactType<MissingQueryProvider>();
    }
}
