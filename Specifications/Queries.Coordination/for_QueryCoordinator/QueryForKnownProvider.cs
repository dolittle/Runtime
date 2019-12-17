// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class QueryForKnownProvider : IQuery
    {
        public QueryType QueryToReturn;
        public bool QueryPropertyCalled;

        public QueryType Query
        {
            get
            {
                QueryPropertyCalled = true;
                return QueryToReturn;
            }
        }
    }
}
