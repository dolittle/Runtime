// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries.Coordination.Specs.for_QueryCoordinator
{
    public class QueryForGenericKnownType : IQuery
    {
        public GenericKnownType<object> QueryToReturn;

        public GenericKnownType<object> Query
        {
            get
            {
                return QueryToReturn;
            }
        }
    }
}
