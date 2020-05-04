// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents a failed <see cref="IProcessingResult" />.
    /// </summary>
    public class FailedProcessing : IProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedProcessing"/> class.
        /// </summary>
        /// <param name="reason">The reason for failure.</param>
        public FailedProcessing(string reason)
        {
            FailureReason = reason;
            Retry = false;
            RetryTimeout = TimeSpan.Zero;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FailedProcessing"/> class.
        /// </summary>
        /// <param name="reason">The reason for failure.</param>
        /// <param name="retry">Whether to retry processing.</param>
        /// <param name="retryTimeout">The retry timeout <see cref="TimeSpan" />.</param>
        public FailedProcessing(string reason, bool retry, TimeSpan retryTimeout)
        {
            FailureReason = reason;
            Retry = retry;
            RetryTimeout = retryTimeout;
        }

        /// <inheritdoc/>
        public bool Succeeded { get; }

        /// <inheritdoc/>
        public string FailureReason { get; }

        /// <inheritdoc/>
        public bool Retry { get; }

        /// <inheritdoc/>
        public TimeSpan RetryTimeout { get; }
    }
}
