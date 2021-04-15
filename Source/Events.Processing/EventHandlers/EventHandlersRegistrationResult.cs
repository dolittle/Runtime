// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Rudimentary;
using Dolittle.Runtime.Events.Processing.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the registration result of an Event Handler.
    /// </summary>
    public class EventHandlersRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersRegistrationResult"/> class.
        /// </summary>
        /// <param name="eventProcessorRegistrationResult">The unsuccessfull <see cref="EventProcessorRegistrationResult" />.</param>
        public EventHandlersRegistrationResult(EventProcessorRegistrationResult eventProcessorRegistrationResult)
        {
            EventProcessorRegistrationResult = eventProcessorRegistrationResult;
            TryRegisterFilter = Try<FilterRegistrationResult>.Failed();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlersRegistrationResult"/> class.
        /// </summary>
        /// <param name="eventProcessorRegistrationResult">The <see cref="EventProcessorRegistrationResult" />.</param>
        /// <param name="filterRegistrationResult">The <see cref="TryRegisterFilter" />.</param>
        public EventHandlersRegistrationResult(EventProcessorRegistrationResult eventProcessorRegistrationResult, FilterRegistrationResult filterRegistrationResult)
        {
            EventProcessorRegistrationResult = eventProcessorRegistrationResult;
            TryRegisterFilter = filterRegistrationResult;
        }

        /// <summary>
        /// Gets the <see cref="Processing.EventProcessorRegistrationResult" />.
        /// </summary>
        public EventProcessorRegistrationResult EventProcessorRegistrationResult { get; }

        /// <summary>
        /// Gets the <see cref="Try{T}" /> with a <see cref="FilterRegistrationResult" /> result.
        /// </summary>
        public Try<FilterRegistrationResult> TryRegisterFilter { get; }

        /// <summary>
        /// Gets a value indicating whether the Event Handler registration succeeded.
        /// </summary>
        public bool Success => EventProcessorRegistrationResult.Success
                                && TryRegisterFilter.Success
                                && TryRegisterFilter.Result.Success;
    }
}
