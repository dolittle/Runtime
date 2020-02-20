// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Artifacts;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="MongoDB.Events.StreamEvent" />.
    /// </summary>
    public static class StreamEventExtensions
    {
        /// <summary>
        /// Gets the <see cref="StreamEventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <returns>The converted <see cref="StreamEventMetadata" />.</returns>
        public static StreamEventMetadata GetStreamEventMetadata(this CommittedEvent committedEvent, PartitionId partition) =>
            new StreamEventMetadata(
                committedEvent.EventLogVersion,
                committedEvent.Occurred,
                partition.Value,
                committedEvent.EventSource,
                committedEvent.CorrelationId,
                committedEvent.Microservice,
                committedEvent.Tenant,
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
        public static CommittedAggregateEvent ToCommittedAggregateEvent(this MongoDB.Events.StreamEvent @event) =>
            new CommittedAggregateEvent(
                new Artifact(@event.Aggregate.TypeId, @event.Aggregate.TypeGeneration),
                @event.Aggregate.Version,
                @event.Metadata.EventLogVersion,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.Metadata.Correlation,
                @event.Metadata.Microservice,
                @event.Metadata.Tenant,
                new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                @event.Metadata.Public,
                @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="MongoDB.Events.StreamEvent" /> to <see cref="CommittedEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="MongoDB.Events.StreamEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this MongoDB.Events.StreamEvent @event) =>
            @event.Aggregate.WasAppliedByAggregate ?
                @event.ToCommittedAggregateEvent()
                : new CommittedEvent(
                    @event.Metadata.EventLogVersion,
                    @event.Metadata.Occurred,
                    @event.Metadata.EventSource,
                    @event.Metadata.Correlation,
                    @event.Metadata.Microservice,
                    @event.Metadata.Tenant,
                    new Cause(@event.Metadata.CauseType, @event.Metadata.CausePosition),
                    new Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                    @event.Metadata.Public,
                    @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="Event" /> to <see cref="StreamEvent" />.
        /// </summary>
        /// <param name="event">The <see cref="Event" />.</param>
        /// <param name="stream">The <see cref="StreamId" />.</param>
        /// <returns>The converted <see cref="StreamEvent" />.</returns>
        public static Streams.StreamEvent ToRuntimeStreamEvent(this MongoDB.Events.StreamEvent @event, StreamId stream) =>
            new Streams.StreamEvent(@event.ToCommittedEvent(), stream, @event.Metadata.Partition);

        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to <see cref="Event" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="partition">The <see cref="PartitionId" />.</param>
        /// <returns>The converted <see cref="Event" />.</returns>
        public static MongoDB.Events.StreamEvent ToStoreStreamEvent(this CommittedEvent committedEvent, StreamPosition streamPosition, PartitionId partition) =>
            new MongoDB.Events.StreamEvent(
                streamPosition,
                committedEvent.GetStreamEventMetadata(partition),
                committedEvent.GetAggregateMetadata(),
                BsonDocument.Parse(committedEvent.Content));
    }
}