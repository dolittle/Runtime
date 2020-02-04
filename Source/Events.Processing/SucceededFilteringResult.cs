// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterResult" /> where filtering succeeded.
    /// </summary>
    public class SucceededFilteringResult : IFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SucceededFilteringResult"/> class.
        /// </summary>
        /// <param name="isIncluded">Is event included in filter.</param>
        /// <param name="partition">The partition.</param>
        public SucceededFilteringResult(bool isIncluded, int partition = 0)
        {
            IsIncluded = isIncluded;
            Partition = partition;
        }

        /// <inheritdoc />
        public bool Succeeded => true;

        /// <inheritdoc />
        public bool Retry => false;

        /// <inheritdoc />
        public bool IsIncluded { get; }

        /// <inheritdoc />
        public int Partition { get; }
    }
}