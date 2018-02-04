/*---------------------------------------------------------------------------------------------
 *  Copyright (c) 2008-2017 doLittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System.Linq;

namespace doLittle.Queries
{
    /// <summary>
    /// Represents an implementation of a <see cref="IQueryProviderFor{T}"/> for <see cref="IQueryable"/>
    /// </summary>
    public class QueryableProvider : IQueryProviderFor<IQueryable>
    {
        /// <inheritdoc/>
        public QueryProviderResult Execute(IQueryable query, PagingInfo paging)
        {
            var result = new QueryProviderResult();

            result.TotalItems = query.Count();

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
