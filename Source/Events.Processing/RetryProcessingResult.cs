// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="ProcessingResult" /> where processing failed and it should try to process again.
    /// </summary>
    public class RetryProcessingResult : ProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetryProcessingResult"/> class.
        /// </summary>
        /// <param name="retryTimeout">The retry timeout.</param>
        public RetryProcessingResult(ulong retryTimeout)
            : base(ProcessingResultValue.Retry) => RetryTimeout = retryTimeout;

        /// <summary>
        /// Gets the retry timeout value.
        /// </summary>
        public ulong RetryTimeout { get; }
    }
}