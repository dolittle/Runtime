// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents an implementation of <see cref="IProcessingResult" /> where processing failed.
    /// </summary>
    public class FailedProcessingResult : IProcessingResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailedProcessingResult"/> class.
        /// </summary>
        /// <param name="failureReason">The reason for failure.</param>
        public FailedProcessingResult(string failureReason) => FailureReason = failureReason;

        /// <inheritdoc/>
        public string FailureReason { get; }

        /// <inheritdoc />
        public bool Succeeded => false;

        /// <inheritdoc />
        public bool Retry => false;
    }
}