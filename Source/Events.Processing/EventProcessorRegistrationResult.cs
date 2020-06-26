// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the registration result of an Event Processor.
    /// </summary>
    public class EventProcessorRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistrationResult"/> class.
        /// </summary>
        /// <param name="success">Whether the <see cref="StreamProcessor" /> was successfully registered.</param>
        /// <param name="streamProcessor">The <see cref="StreamProcessor" />.</param>
        public EventProcessorRegistrationResult(bool success, StreamProcessor streamProcessor)
        {
            Success = success;
            StreamProcessor = streamProcessor;
        }

        /// <summary>
        /// Gets the <see cref="StreamProcessor" />.
        /// </summary>
        public StreamProcessor StreamProcessor { get; }

        /// <summary>
        /// Gets a value indicating whether the stream processor was registered successfully.
        /// </summary>
        public bool Success { get; }
    }
}
