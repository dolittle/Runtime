// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Runtime.Events.Processing.Filters;
using Dolittle.Runtime.Events.Processing.Streams;
using Dolittle.Runtime.Events.Store.Streams.Filters;

namespace Dolittle.Runtime.Events.Processing.EventHandlers
{
    /// <summary>
    /// Represents the registration result of the filter and event handler <see cref="StreamProcessor "/> for an event handler.
    /// </summary>
    public class EventHandlerRegistrationResult
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistrationResult"/> class.
        /// </summary>
        /// <param name="filterStreamProcessor">The <see cref="StreamProcessor" /> for the filter.</param>
        /// <param name="eventHandlerStreamProcessor">The <see cref="StreamProcessor" /> for the event handler.</param>
        /// <param name="filterProcessor">The <see cref="IFilterProcessor{T}" />.</param>
        public EventHandlerRegistrationResult(StreamProcessor filterStreamProcessor, StreamProcessor eventHandlerStreamProcessor, IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> filterProcessor)
        {
            Succeeded = true;
            FilterStreamProcessor = filterStreamProcessor;
            EventHandlerStreamProcessor = eventHandlerStreamProcessor;
            FilterProcessor = filterProcessor;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EventHandlerRegistrationResult"/> class.
        /// </summary>
        /// <param name="failureReason">The <see cref="FailedEventHandlerRegistrationReason" />.</param>
        public EventHandlerRegistrationResult(FailedEventHandlerRegistrationReason failureReason)
        {
            Succeeded = false;
            FailureReason = failureReason;
        }

        /// <summary>
        /// Gets a value indicating whether the registration of the <see cref="StreamProcessor">stream processors</see> for the event handler succeeded.
        /// </summary>
        public bool Succeeded { get; }

        /// <summary>
        /// Gets the filter <see cref="StreamProcessor" />.
        /// </summary>
        public StreamProcessor FilterStreamProcessor { get; }

        /// <summary>
        /// Gets the event handler <see cref="StreamProcessor" />.
        /// </summary>
        public StreamProcessor EventHandlerStreamProcessor { get; }

        /// <summary>
        /// Gets the <see cref="IFilterProcessor{T}" /> for <see cref="TypeFilterWithEventSourcePartitionDefinition" />.
        /// </summary>
        public IFilterProcessor<TypeFilterWithEventSourcePartitionDefinition> FilterProcessor { get; }

        /// <summary>
        /// Gets the reason for why the registration failed.
        /// </summary>
        public FailedEventHandlerRegistrationReason FailureReason { get; }
    }
}
