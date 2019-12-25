// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class QueryProviderForDerivedType : IQueryProviderFor<DerivedQueryType>
    {
        public QueryProviderResult result_to_return;

        public bool execute_called;

        public DerivedQueryType query_passed_to_execute;

        public PagingInfo paging_info_passed_to_execute;

        public QueryProviderResult Execute(DerivedQueryType query, PagingInfo paging)
        {
            query_passed_to_execute = query;
            paging_info_passed_to_execute = paging;
            execute_called = true;

            return result_to_return;
        }
    }
}
