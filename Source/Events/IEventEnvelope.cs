/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Dolittle. All rights reserved.
 *  Licensed under the MIT License. See LICENSE in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using Dolittle.Applications;
using Dolittle.Events;
using Dolittle.Artifacts;
using Dolittle.Execution;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Defines the envelope for the event with all the metadata related to the event
    /// </summary>
    public interface IEventEnvelope
    {
        /// <summary>
        /// Gets the <see cref="CorrelationId"/> that the <see cref="IEvent"/> is part of
        /// </summary>
        CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="EventId"/> representing the <see cref="IEvent"/>s
        /// </summary>
        EventId EventId { get; }

        /// <summary>
        /// Gets the global sequence number used in the event store
        /// </summary>
        EventSequenceNumber SequenceNumber { get; }

        /// <summary>
        /// Gets the global sequence number for the specific <see cref="IEvent">event type</see>
        /// </summary>
        EventSequenceNumber SequenceNumberForEventType { get; }


        /// <summary>
        /// Gets the <see cref="Generation"/> for the <see cref="IEvent"/>
        /// </summary>
        Generation Generation { get; }

        /// <summary>
        /// Gets the <see cref="Artifact">identifier</see> identifying the <see cref="IEvent"/>
        /// </summary>
        Artifact Event { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceId">id</see> of the <see cref="IEventSource"/>
        /// </summary>
        EventSourceId EventSourceId { get; }

        /// <summary>
        /// Gets the <see cref="Artifact">identifier</see> identifying the <see cref="IEventSource"/>
        /// </summary>
        Artifact EventSource { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceVersion">version</see> of the <see cref="IEventSource"/>
        /// </summary>
        EventSourceVersion Version { get; }

        /// <summary>
        /// Gets who or what the event was caused by.
        /// 
        /// Typically this would be the name of the user or system causing it
        /// </summary>
        CausedBy CausedBy { get; }

        /// <summary>
        /// Gets the time the event occurred
        /// </summary>
        DateTimeOffset Occurred { get; }

        /// <summary>
        /// Creates a new <see cref="EventEnvelope"/> with a different <see cref="CorrelationId">correlation id</see>
        /// </summary>
        /// <param name="correlationId"></param>
        /// <returns>A copy of the <see cref="EventEnvelope"/> with a new <see cref="CorrelationId"/> </returns>
        IEventEnvelope WithTransactionCorrelationId(CorrelationId correlationId);

        /// <summary>
        /// Creates a new <see cref="EventEnvelope"/> with a different <see cref="EventSequenceNumber">sequence number</see>
        /// </summary>
        /// <param name="sequenceNumber">The new <see cref="EventSequenceNumber"/></param>
        /// <returns>A copy of the <see cref="EventEnvelope"/> with a new Id </returns>
        IEventEnvelope WithSequenceNumber(EventSequenceNumber sequenceNumber);

        /// <summary>
        /// Creates a new <see cref="EventEnvelope"/> with a different <see cref="EventSequenceNumber">sequence number</see> for the <see cref="IEvent">event type</see>
        /// </summary>
        /// <param name="sequenceNumber">The new <see cref="EventSequenceNumber"/></param>
        /// <returns>A copy of the <see cref="EventEnvelope"/> with a new Id </returns>
        IEventEnvelope WithSequenceNumberForEventType(EventSequenceNumber sequenceNumber);
    }
}