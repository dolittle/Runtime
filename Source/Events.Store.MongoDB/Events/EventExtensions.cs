// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Store.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="Event" />.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Gets the <see cref="EventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <returns>The converted <see cref="EventMetadata" />.</returns>
        public static EventMetadata GetEventMetadata(this CommittedEvent committedEvent) =>
            new EventMetadata(
                committedEvent.Occurred,
                committedEvent.EventSource,
                committedEvent.Cause.Type,
                committedEvent.Cause.Position,
                committedEvent.Type.Id,
                committedEvent.Type.Generation,
                committedEvent.Public);

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedAggregateEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedAggregateEvent" />.</returns>
        public static CommittedAggregateEvent ToCommittedAggregateEvent(this Event @event) =>
            new CommittedAggregateEvent(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.EventLogSequenceNumber,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.ExecutionContext.Correlation,
                @event.ExecutionContext.Microservice,
                @event.ExecutionContext.Tenant,
                new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this Event @event) =>
            @event.Aggregate.WasAppliedByAggregate ?
                @event.ToCommittedAggregateEvent()
                : new CommittedEvent(
                      @event.EventLogSequenceNumber,
                      @event.Metadata.Occurred,
                      @event.Metadata.EventSource,
                      @event.ExecutionContext.Correlation,
                      @event.ExecutionContext.Microservice,
                      @event.ExecutionContext.Tenant,
                      new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                      new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                      @event.Metadata.Public,
                      @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to a <see cref="Runtime.Events.Store.Streams.StreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Store.Streams.StreamEvent" />.</returns>
        public static Runtime.Events.Store.Streams.StreamEvent ToRuntimeStreamEvent(this Event @event) =>
            new Runtime.Events.Store.Streams.StreamEvent(@event.ToCommittedEvent(), @event.EventLogSequenceNumber, StreamId.AllStreamId, PartitionId.NotSet);
    }
}