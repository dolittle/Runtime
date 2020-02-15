// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterResult" /> where filtering failed.
    /// </summary>
    public class FailedFilteringResult : IFilterResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedFilteringResult"/> class.
        /// </summary>
        /// <param name="failureReason">The reason for failure.</param>
        public FailedFilteringResult(string failureReason) => FailureReason = failureReason;

        /// <inheritdoc/>
        public string FailureReason { get; }

        /// <inheritdoc />
        public bool Succeeded => false;

        /// <inheritdoc />
        public bool Retry => false;

        /// <inheritdoc />
        public bool IsIncluded => false;

        /// <inheritdoc />
        public PartitionId Partition => PartitionId.NotSet;
    }
}