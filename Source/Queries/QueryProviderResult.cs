// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections;

namespace Dolittle.Queries
{
    /// <summary>
    /// Represents the result of issuing a query for a provider.
    /// </summary>
    public class QueryProviderResult
    {
        /// <summary>
        /// Gets or sets the count of total items from a query.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the items as the result of a query.
        /// </summary>
        public IEnumerable Items { get; set; }
    }
}
