// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Concepts;
using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the reason for why the registration of an event handler failed.
    /// </summary>
    public class FailedEventHandlerRegistrationReason : ConceptAs<string>
    {
        /// <summary>
        /// Gets a value indicating whether the <see cref="FailedEventHandlerRegistrationReason" /> is set.
        /// </summary>
        /// <returns>True if set, false if not.</returns>
        public bool IsSet => !string.IsNullOrEmpty(Value);

        /// <summary>
        /// Creates a <see cref="FailedEventHandlerRegistrationReason" /> from <see cref="StreamProcessorRegistration" /> and <see cref="FilterValidationResult" />.
        /// </summary>
        /// <param name="eventProcessorRegistrationResult">The <see cref="StreamProcessorRegistration" /> for the event processor <see cref="StreamProcessor" />.</param>
        /// <param name="filterProcessorRegistrationResult">The <see cref="StreamProcessorRegistration" /> for the filter <see cref="StreamProcessor" />.</param>
        /// <param name="filterValidationResult">The <see cref="FilterValidationResult" />.</param>
        /// <returns>The <see cref="FailedEventHandlerRegistrationReason" />.</returns>
        public static FailedEventHandlerRegistrationReason FromRegistrationResults(
            StreamProcessorRegistration eventProcessorRegistrationResult,
            StreamProcessorRegistration filterProcessorRegistrationResult,
            FilterValidationResult filterValidationResult)
        {
            var value = string.Empty;
            value += eventProcessorRegistrationResult.Failed ?
                string.Empty
                : $"{(string.IsNullOrEmpty(value) ? string.Empty : "\n")}Stream Processor for event processor with Stream Processor Id: '{eventProcessorRegistrationResult.StreamProcessor.Identifier}' has already been registered";
            value += filterProcessorRegistrationResult.Failed ?
                string.Empty
                : $"{(string.IsNullOrEmpty(value) ? string.Empty : "\n")}Stream Processor for filter with Stream Processor Id: '{filterProcessorRegistrationResult.StreamProcessor.Identifier}' has already been registered";

            value += filterValidationResult.Succeeded ?
                string.Empty
                : $"{(string.IsNullOrEmpty(value) ? string.Empty : "\n")}Filter validation failed. {filterValidationResult.FailureReason}";

            return new FailedEventHandlerRegistrationReason { Value = value };
        }
    }
}
