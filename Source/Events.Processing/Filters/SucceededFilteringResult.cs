// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
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
        /// <param name="partitionId">The <see cref="PartitionId" /> />.</param>
        public SucceededFilteringResult(bool isIncluded, PartitionId partitionId)
        {
            IsIncluded = isIncluded;
            Partition = partitionId;
        }

        /// <inheritdoc />
        public bool Succeeded => true;

        /// <inheritdoc />
        public bool Retry => false;

        /// <inheritdoc />
        public bool IsIncluded { get; }

        /// <inheritdoc />
        public PartitionId Partition { get; }
    }
}