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
        /// <param name="newStreamProcessorWasRegistered">Whether a new <see cref="StreamProcessor" /> was registered.</param>
        /// <param name="streamProcessor">The <see cref="StreamProcessor" />.</param>
        public StreamProcessorRegistrationResult(bool newStreamProcessorWasRegistered, StreamProcessor streamProcessor)
        {
            NewStreamProcessorWasRegistered = newStreamProcessorWasRegistered;
            StreamProcessor = streamProcessor;
        }

        /// <summary>
        /// Gets a value indicating whether a new <see cref="StreamProcessor" /> was registered.
        /// </summary>
        public bool NewStreamProcessorWasRegistered { get; }

        /// <summary>
        /// Gets the registered stream processor.
        /// </summary>
        public StreamProcessor StreamProcessor { get; }
    }
}