// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Dolittle.Artifacts;
using Dolittle.Concepts;
using Dolittle.Events;
using Dolittle.Execution;
using Dolittle.Time;

namespace Dolittle.Runtime.Events
{
    /// <summary>
    /// Represents the metadata associated with a particular <see cref="IEvent" /> instance>
    /// Exhibits value equality semantics.
    /// </summary>
    public class EventMetadata : Value<EventMetadata>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventMetadata"/> class.
        /// </summary>
        /// <param name="eventId">An <see cref="EventId"/> that uniquely identifies this event.</param>
        /// <param name="versionedEventSource">The <see cref="VersionedEventSource" /> that this event applies to.</param>
        /// <param name="correlationId">A <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system.</param>
        /// <param name="artifact">The <see cref="Artifact" /> that represents this event.</param>
        /// <param name="occurred">A timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.</param>
        /// <param name="originalContext">The <see cref="OriginalContext" /> of the <see cref="IEvent" />.</param>
        public EventMetadata(EventId eventId, VersionedEventSource versionedEventSource, CorrelationId correlationId, Artifact artifact, DateTimeOffset occurred, OriginalContext originalContext)
        {
            Id = eventId;
            VersionedEventSource = versionedEventSource;
            CorrelationId = correlationId;
            Artifact = artifact;
            Occurred = occurred;
            OriginalContext = originalContext;
        }

        /// <summary>
        /// Gets the <see cref="EventId" /> that uniquely identifies this Event.
        /// </summary>
        public EventId Id { get; }

        /// <summary>
        /// Gets the <see cref="VersionedEventSource" /> that this event applies to.
        /// </summary>
        public VersionedEventSource VersionedEventSource { get; }

        /// <summary>
        /// Gets the <see cref="CorrelationId" /> to relate this event to other artifacts and actions within the system.
        /// </summary>
        public CorrelationId CorrelationId { get; }

        /// <summary>
        /// Gets The <see cref="Artifact" /> that the event represents.
        /// </summary>
        public Artifact Artifact { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceId" /> identifying the <see cref="IEventSource" /> that this event applies to.
        /// </summary>
        public EventSourceId EventSourceId => VersionedEventSource != null ? VersionedEventSource.EventSource : EventSourceId.Empty;

        /// <summary>
        /// gets the <see cref="OriginalContext" /> derived from the <see cref="ExecutionContext" /> when this <see cref="IEvent" /> occurred.
        /// </summary>
        public OriginalContext OriginalContext { get; }

        /// <summary>
        /// Gets the timestamp in the form of a <see cref="DateTimeOffset" /> recording when the <see cref="IEvent" /> occurred.
        /// </summary>
        public DateTimeOffset Occurred { get; }

        /// <summary>
        /// Gets the <see cref="EventSourceKey" /> from this EventMetadata.
        /// </summary>
        /// <returns>the <see cref="EventSourceKey" />.</returns>
        public EventSourceKey EventSourceKey => VersionedEventSource.Key;

        /// <inheritdoc/>
        public override bool Equals(EventMetadata other)
        {
            return
                    Id.Equals(other.Id)
                && Artifact.Equals(other.Artifact)
                && CorrelationId.Equals(other.CorrelationId)
                && EventSourceId.Equals(other.EventSourceId)
                && Occurred.LossyEquals(other.Occurred)
                && OriginalContext.Equals(other.OriginalContext)
                && VersionedEventSource.Equals(other.VersionedEventSource);
        }
    }
}