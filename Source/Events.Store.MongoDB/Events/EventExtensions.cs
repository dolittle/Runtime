// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Streams;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="Event" />.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="CommittedAggregateEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="CommittedAggregateEvent" />.</returns>
        public static CommittedAggregateEvent ToCommittedAggregateEvent(this Event @event) =>
            new CommittedAggregateEvent(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.EventLogVersion,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.Metadata.Correlation,
                @event.Metadata.Microservice,
                @event.Metadata.Tenant,
                new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Public,
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
                      @event.EventLogVersion,
                      @event.Metadata.Occurred,
                      @event.Metadata.EventSource,
                      @event.Metadata.Correlation,
                      @event.Metadata.Microservice,
                      @event.Metadata.Tenant,
                      new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                      new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                      @event.Public,
                      @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to a <see cref="Streams.StreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <returns>The converted <see cref="Streams.StreamEvent" />.</returns>
        public static Streams.StreamEvent ToRuntimeStreamEvent(this Event @event) =>
            new Streams.StreamEvent(@event.ToCommittedEvent(), StreamId.AllStreamId, PartitionId.NotSet);
    }
}