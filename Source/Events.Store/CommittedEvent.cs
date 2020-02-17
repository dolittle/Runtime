// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Applications;
using Dolittle.Artifacts;
using Dolittle.Execution;
using Dolittle.Tenancy;

namespace Dolittle.Runtime.Events.Store
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
        /// <param name="eventSource">The <see cref="EventSource" />.</param>
        /// <param name="correlationId">The <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system.</param>
        /// <param name="microservice">The <see cref="Microservice"/> within which the Event occurred.</param>
        /// <param name="tenant">The <see cref="TenantId"/> within which the Event occurred.</param>
        /// <param name="cause">The link to the cause of the Event.</param>
        /// <param name="type">The <see cref="Artifact"/> representing the type of the Event.</param>
        /// <param name="isPublic">Whether this Event is public.</param>
        /// <param name="content">The content of the Event represented as a JSON-encoded <see cref="string"/>.</param>
        public CommittedEvent(
            EventLogVersion eventLogVersion,
            DateTimeOffset occurred,
            EventSourceId eventSource,
            CorrelationId correlationId,
            Microservice microservice,
            TenantId tenant,
            Cause cause,
            Artifact type,
            bool isPublic,
            string content)
        {
            EventLogVersion = eventLogVersion;
            Occurred = occurred;
            EventSource = eventSource;
            CorrelationId = correlationId;
            Microservice = microservice;
            Tenant = tenant;
            Cause = cause;
            Type = type;
            Public = isPublic;
            Content = content;
        }

        /// <summary>
        /// Gets the <see cref="EventSource" />. that the Event was applied to.
        /// </summary>
        public EventSourceId EventSource { get; }

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
        /// Gets the <see cref="Artifact"/> representing the type of the Event.
        /// </summary>
        public Artifact Type { get; }

        /// <summary>
        /// Gets a value indicating whether the Event is public.
        /// </summary>
        public bool Public { get; }

        /// <summary>
        /// Gets the content of the Event represented as a JSON-encoded <see cref="string"/>.
        /// </summary>
        public string Content { get; }
    }
}