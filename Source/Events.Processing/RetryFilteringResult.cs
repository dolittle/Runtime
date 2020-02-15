// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IFilterResult" /> where filtering failed and it should try to filter again.
    /// </summary>
    public class RetryFilteringResult : IRetryFilteringResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryFilteringResult"/> class.
        /// </summary>
        /// <param name="retryTimeout">The retry timeout in milliseconds.</param>
        /// <param name="failureReason">The reason for failure.</param>
        public RetryFilteringResult(uint retryTimeout, string failureReason)
        {
            RetryTimeout = retryTimeout;
            FailureReason = failureReason;
        }

        /// <inheritdoc/>
        public string FailureReason { get; }

        /// <inheritdoc />
        public bool Succeeded => false;

        /// <inheritdoc />
        public bool Retry => true;

        /// <inheritdoc />
        public bool IsIncluded => false;

        /// <inheritdoc />
        public PartitionId Partition => PartitionId.NotSet;

        /// <inheritdoc/>
        public uint RetryTimeout { get; }
    }
}