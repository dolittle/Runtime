// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProcessingResult" /> where processing failed and it should try to process again.
    /// </summary>
    public class RetryProcessingResult : IRetryProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryProcessingResult"/> class.
        /// </summary>
        /// <param name="retryTimeout">The retry timeout in milliseconds.</param>
        /// <param name="failureReason">The reason for failure.</param>
        public RetryProcessingResult(uint retryTimeout, string failureReason)
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

        /// <inheritdoc/>
        public uint RetryTimeout { get; }
    }
}