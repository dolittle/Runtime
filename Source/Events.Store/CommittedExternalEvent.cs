// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Runtime.Events.Store.EventHorizon;

namespace Dolittle.Runtime.Events.Store
{
    /// <summary>
    /// Represents an Event that was received over the Event Horizon from an external Microservice.
    /// </summary>
    public class CommittedExternalEvent : CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedExternalEvent"/> class.
        /// </summary>
        /// <param name="eventLogSequenceNumber">The <see cref="EventLogSequenceNumber" />.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> of when the Event was committed.</param>
        /// <param name="eventSource">The <see cref="EventSourceId" />.</param>
        /// <param name="executionContext">THe <see cref="ExecutionContext" />.</param>
        /// <param name="type">The <see cref="Artifact" />.</param>
        /// <param name="isPublic">Whether the event is public.</param>
        /// <param name="content">The Event content.</param>
        /// <param name="externalEventLogSequenceNumber">The external <see cref="EventLogSequenceNumber" />.</param>
        /// <param name="received">The <see cref="DateTimeOffset" /> that this external Event was received.</param>
        /// <param name="consent">The <see cref="Consent" />.</param>
        public CommittedExternalEvent(
            EventLogSequenceNumber eventLogSequenceNumber,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            ExecutionContext executionContext,
            Artifact type,
            bool isPublic,
            string content,
            EventLogSequenceNumber externalEventLogSequenceNumber,
            DateTimeOffset received,
            ConsentId consent)
            : base(eventLogSequenceNumber, occurred, eventSource, executionContext, type, isPublic, content)
        {
            ExternalEventLogSequenceNumber = externalEventLogSequenceNumber;
            Received = received;
            Consent = consent;
        }

        /// <summary>
        /// Gets the external <see cref="EventLogSequenceNumber" />.
        /// </summary>
        public EventLogSequenceNumber ExternalEventLogSequenceNumber { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> for when this external Event was received.
        /// </summary>
        public DateTimeOffset Received { get; }

        /// <summary>
        /// Gets the <see cref="ConsentId" />.
        /// </summary>
        public ConsentId Consent { get; }
    }
}
