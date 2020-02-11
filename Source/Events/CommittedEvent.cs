// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represent an Event that is committed to the Event Store.
    /// </summary>
    public class CommittedEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CommittedEvent"/> class.
        /// </summary>
        /// <param name="eventLogVersion">The version of the Event Log the Event was committed to.</param>
        /// <param name="occurred">The <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.</param>
        /// <param name="correlationId">The <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system.</param>
        /// <param name="microservice">The <see cref="Microservice"/> within which the Event occured.</param>
        /// <param name="tenant">The <see cref="TenantId"/> within which the Event occurred.</param>
        /// <param name="cause">The link to the cause of the Event.</param>
        /// <param name="event">An instance of the Event that was committed to the Event Store.</param>
        public CommittedEvent(EventLogVersion eventLogVersion, DateTimeOffset occurred, CorrelationId correlationId, Microservice microservice, TenantId tenant, Cause cause, IEvent @event)
        {
            EventLogVersion = eventLogVersion;
            Occurred = occurred;
            CorrelationId = correlationId;
            Microservice = microservice;
            Tenant = tenant;
            Cause = cause;
            Event = @event;
        }

        /// <summary>
        /// Gets the version of the Event Log the Event was committed to.
        /// </summary>
        public EventLogVersion EventLogVersion { get; }

        /// <summary>
        /// Gets the <see cref="DateTimeOffset" /> when the Event was committed to the Event Store.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// Gets the <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets the <see cref="Microservice"/> within which the Event occurred.
        /// </summary>
        public Microservice Microservice { get; }

        /// <summary>
        /// Gets the <see cref="TenantId"/> within which the Event occurred.
        /// </summary>
        public TenantId Tenant { get; }

        /// <summary>
        /// Gets the link to the cause of the Event.
        /// </summary>
        public Cause Cause { get; }

        /// <summary>
        /// Gets an instance of the Event that was committed to the Event Store.
        /// </summary>
        public IEvent Event { get; }
    }
}