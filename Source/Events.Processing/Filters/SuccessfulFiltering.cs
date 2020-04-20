// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Processing.Filters
{
    /// <summary>
    /// Represents a successful <see cref="IFilterResult" />.
    /// </summary>
    public class SuccessfulFiltering : SuccessfulProcessing, IFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuccessfulFiltering"/> class.
        /// </summary>
        /// <param name="isIncluded">Is event included in filter.</param>
        /// <param name="partitionId">The <see cref="PartitionId" /> />.</param>
        public SuccessfulFiltering(bool isIncluded, PartitionId partitionId)
        {
            IsIncluded = isIncluded;
            Partition = partitionId;
        }

        /// <inheritdoc />
        public bool IsIncluded { get; }

        /// <inheritdoc />
        public PartitionId Partition { get; }
    }
}
