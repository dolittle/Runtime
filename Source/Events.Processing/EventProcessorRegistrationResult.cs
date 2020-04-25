// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the registration result of the filter and event handler <see cref="StreamProcessor "/> for an event handler.
    /// </summary>
    public class EventProcessorRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistrationResult"/> class.
        /// </summary>
        public EventProcessorRegistrationResult()
        {
            Succeeded = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorRegistrationResult"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="FailedEventProcessorRegistrationReason" />.</param>
        public EventProcessorRegistrationResult(FailedEventProcessorRegistrationReason failureReason)
        {
            Succeeded = false;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether the registration of the <see cref="StreamProcessor">stream processors</see> for the Event Processor succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="FailedEventProcessorRegistrationReason" />.
        /// </summary>
        public FailedEventProcessorRegistrationReason FailureReason { get; }
    }
}
