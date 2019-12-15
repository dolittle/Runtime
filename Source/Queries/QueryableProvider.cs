// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Linq;

namespace Dolittle.Queries
{
    /// <summary>
    /// Represents an implementation of a <see cref="IQueryProviderFor{T}"/> for <see cref="IQueryable"/>.
    /// </summary>
    public class QueryableProvider : IQueryProviderFor<IQueryable>
    {
        /// <inheritdoc/>
        public QueryProviderResult Execute(IQueryable query, PagingInfo paging)
        {
            var result = new QueryProviderResult
            {
                TotalItems = query.Count()
            };

            if (paging.Enabled)
            {
                var start = paging.Size * paging.Number;
                var end = paging.Size;
                if (query.IsTakeEndIndex()) end += start;
                query = query.Skip(start).Take(end);
            }

            result.Items = query;

            return result;
        }
    }
}
