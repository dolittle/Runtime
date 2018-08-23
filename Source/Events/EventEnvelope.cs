/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Events;
using Dolittle.Applications;
using Dolittle.Runtime.Transactions;
using Dolittle.Artifacts;
using Dolittle.Execution;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents an implementation of <see cref="IEventEnvelope"/>; the envelope for the event with all the metadata related to the event
    /// </summary>
    public class EventEnvelope : IEventEnvelope
    {
        /// <summary>
        /// Initializes a new instance of <see cref="EventEnvelope"/>
        /// </summary>
        /// <param name="correlationId"><see cref="CorrelationId"/> the <see cref="IEvent"/> is part of</param>
        /// <param name="eventId"><see cref="EventId"/> for the <see cref="IEvent"/></param>
        /// <param name="sequenceNumber"></param>
        /// <param name="sequenceNumberForEventType"></param>
        /// <param name="generation"><see cref="Generation"/> for the <see cref="IEvent"/> </param>
        /// <param name="event"><see cref="Artifact"/> representing the <see cref="IEvent"/></param>
        /// <param name="eventSourceId"><see cref="EventSourceId"/> for the <see cref="IEventSource"/></param>
        /// <param name="eventSource"><see cref="Artifact"/> representing the <see cref="IEventSource"/></param>
        /// <param name="version"><see cref="EventSourceVersion">Version</see> of the event related to the <see cref="IEventSource"/></param>
        /// <param name="causedBy"><see cref="string"/> representing which person or what system caused the event</param>
        /// <param name="occurred"><see cref="DateTime">When</see> the event occured</param>
        public EventEnvelope(
            CorrelationId correlationId,
            EventId eventId,
            EventSequenceNumber sequenceNumber,
            EventSequenceNumber sequenceNumberForEventType,
            Generation generation, 
            Artifact @event, 
            EventSourceId eventSourceId, 
            Artifact eventSource, 
            EventSourceVersion version, 
            CausedBy causedBy, 
            DateTimeOffset occurred)
        {
            CorrelationId = correlationId;
            EventId = eventId;
            SequenceNumber = sequenceNumber;
            SequenceNumberForEventType = sequenceNumberForEventType;
            Generation = generation;
            Event = @event;
            EventSourceId = eventSourceId;
            EventSource = eventSource;
            Version = version;
            CausedBy = causedBy;
            Occurred = occurred;
        }

        /// <inheritdoc/>
        public CorrelationId CorrelationId { get; }

        /// <inheritdoc/>
        public EventId EventId { get; }

        /// <inheritdoc/>
        public EventSequenceNumber SequenceNumber { get; }

        /// <inheritdoc/>
        public EventSequenceNumber SequenceNumberForEventType { get; }

        /// <inheritdoc/>
        public Generation Generation { get; }

        /// <inheritdoc/>
        public Artifact Event { get; }

        /// <inheritdoc/>
        public EventSourceId EventSourceId { get; }

        /// <inheritdoc/>
        public Artifact EventSource { get; }

        /// <inheritdoc/>
        public EventSourceVersion Version { get; }

        /// <inheritdoc/>
        public CausedBy CausedBy { get; }

        /// <inheritdoc/>
        public DateTimeOffset Occurred { get; }

        /// <inheritdoc/>
        public IEventEnvelope WithSequenceNumber(EventSequenceNumber sequenceNumber)
        {
            return new EventEnvelope(CorrelationId, EventId, sequenceNumber, SequenceNumberForEventType, Generation, Event, EventSourceId, EventSource, Version, CausedBy, Occurred);
        }

        /// <inheritdoc/>
        public IEventEnvelope WithSequenceNumberForEventType(EventSequenceNumber sequenceNumberForEventType)
        {
            return new EventEnvelope(CorrelationId, EventId, SequenceNumber, sequenceNumberForEventType, Generation, Event, EventSourceId, EventSource, Version, CausedBy, Occurred);
        }

        /// <inheritdoc/>
        public IEventEnvelope WithTransactionCorrelationId(CorrelationId correlationId)
        {
            return new EventEnvelope(correlationId, EventId, SequenceNumber, SequenceNumberForEventType, Generation, Event, EventSourceId, EventSource, Version, CausedBy, Occurred);
        }
    }
}
