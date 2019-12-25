// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;

namespace Dolittle.Queries.Coordination
{
    /// <summary>
    /// Defines a coordinator of queries.
    /// </summary>
    public interface IQueryCoordinator
    {
        /// <summary>
        /// Execute a <see cref="IQuery"/>.
        /// </summary>
        /// <param name="query"><see cref="IQuery"/> to execute.</param>
        /// <param name="paging"><see cref="PagingInfo"/> applied to the query.</param>
        /// <returns><see cref="QueryResult">Result</see> of the query.</returns>
        Task<QueryResult> Execute(IQuery query, PagingInfo paging);
    }
}
