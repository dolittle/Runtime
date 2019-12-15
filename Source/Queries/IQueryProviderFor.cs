// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries
{
    /// <summary>
    /// Defines a provider that can deal with a query for <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">Type of query the provider is for.</typeparam>
    /// <remarks>
    /// Types inheriting from this interface will be automatically registered and called whenever a <see cref="IQuery"/>
    /// with a Query property of type <typeparamref name="T"/> is encountered.
    /// </remarks>
    public interface IQueryProviderFor<T>
    {
        /// <summary>
        /// Execute a query.
        /// </summary>
        /// <param name="query">Query to execute.</param>
        /// <param name="paging"><see cref="PagingInfo"/> to apply.</param>
        /// <returns><see cref="QueryResult">Result</see> from the query.</returns>
        QueryProviderResult Execute(T query, PagingInfo paging);
    }
}
