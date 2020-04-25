// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing.Streams
{
    /// <summary>
    /// Represents the result of the registration of a stream processor.
    /// </summary>
    public class StreamProcessorRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorRegistrationResult"/> class.
        /// </summary>
        /// <param name="streamProcessor">The <see cref="StreamProcessor" />.</param>
        public StreamProcessorRegistrationResult(StreamProcessor streamProcessor)
        {
            Succeeded = true;
            StreamProcessor = streamProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StreamProcessorRegistrationResult"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="StreamProcessorRegistrationFailureReason" />.</param>
        public StreamProcessorRegistrationResult(StreamProcessorRegistrationFailureReason failureReason)
        {
            Succeeded = false;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether a new <see cref="StreamProcessor" /> was registered.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="StreamProcessorRegistrationFailureReason" />.
        /// </summary>
        public StreamProcessorRegistrationFailureReason FailureReason { get; }

        /// <summary>
        /// Gets the registered stream processor.
        /// </summary>
        public StreamProcessor StreamProcessor { get; }
    }
}
