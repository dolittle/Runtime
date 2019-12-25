// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Queries
{
    /// <summary>
    /// Represents paging that can be added to a query.
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// Gets or sets the size of the pages.
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// Gets or sets the current page number.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        /// Gets a value indicating whether or not Paging is enabled.
        /// </summary>
        public bool Enabled => Size > 0;
    }
}
