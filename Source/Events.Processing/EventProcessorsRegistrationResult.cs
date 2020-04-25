// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Dolittle.Runtime.Events.Processing
{
    /// <summary>
    /// Represents the result of registering Event Processors.
    /// </summary>
    public class EventProcessorsRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorsRegistrationResult"/> class.
        /// </summary>
        public EventProcessorsRegistrationResult()
        {
            Succeeded = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventProcessorsRegistrationResult"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="FailedEventProcessorsRegistrationReason" />.</param>
        public EventProcessorsRegistrationResult(FailedEventProcessorsRegistrationReason failureReason)
        {
            Succeeded = false;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether the registration succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the <see cref="FailedEventProcessorsRegistrationReason" />.
        /// </summary>
        public FailedEventProcessorsRegistrationReason FailureReason { get; }
    }
}
