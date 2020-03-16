// Copyright (c) Dolittle. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Dolittle.Applications;
using Dolittle.Runtime.Events.Streams;
using MongoDB.Bson;

namespace Dolittle.Runtime.Events.Store.MongoDB.Events
{
    /// <summary>
    /// Extension methods for <see cref="EventHorizonEvent" />.
    /// </summary>
    public static class EventHorizonEventExtensions
    {
        /// <summary>
        /// Gets the <see cref="EventHorizonEventMetadata"/> from the <see cref="CommittedEvent"/>.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent"/>.</param>
        /// <param name="eventHorizon">The <see cref="Runtime.EventHorizon.EventHorizon" />.</param>
        /// <returns>The converted <see cref="EventHorizonEventMetadata" />.</returns>
        public static EventHorizonEventMetadata GetEventHorizonEventMetadata(this CommittedEvent committedEvent, Runtime.EventHorizon.EventHorizon eventHorizon) =>
            new EventHorizonEventMetadata(
                committedEvent.Occurred,
                committedEvent.EventSource,
                committedEvent.CorrelationId,
                eventHorizon.ProducerMicroservice,
                eventHorizon.ConsumerTenant,
                eventHorizon.ProducerTenant,
                committedEvent.EventLogSequenceNumber,
                committedEvent.Type.Id,
                committedEvent.Type.Generation);

        /// <summary>
        /// Converts a <see cref="EventHorizonEvent" /> to a <see cref="CommittedEvent" /> for the .
        /// </summary>
        /// <param name="event">The <see cref="EventHorizonEvent" />.</param>
        /// <returns>The converted <see cref="CommittedEvent" />.</returns>
        public static CommittedEvent ToCommittedEvent(this EventHorizonEvent @event) =>
            new CommittedEvent(
                @event.StreamPosition,
                @event.Metadata.Occurred,
                @event.Metadata.EventSource,
                @event.Metadata.Correlation,
                @event.Metadata.Microservice,
                @event.Metadata.ConsumerTenant,
                new Cause(CauseType.Event, 0),
                new Artifacts.Artifact(@event.Metadata.TypeId, @event.Metadata.TypeGeneration),
                false,
                @event.Content.ToString());

        /// <summary>
        /// Converts a <see cref="EventHorizonEvent" /> to a <see cref="Runtime.Events.Streams.StreamEvent" />.
        /// </summary>
        /// <param name="eventHorizonEvent">The <see cref="EventHorizonEvent" />.</param>
        /// <param name="microservice">The <see cref="Microservice" />.</param>
        /// <returns>The converted <see cref="Runtime.Events.Streams.StreamEvent" />.</returns>
        public static Runtime.Events.Streams.StreamEvent ToRuntimeStreamEvent(this EventHorizonEvent eventHorizonEvent, Microservice microservice) =>
            new Runtime.Events.Streams.StreamEvent(eventHorizonEvent.ToCommittedEvent(), microservice.Value, PartitionId.NotSet);

        /// <summary>
        /// Converts a <see cref="CommittedEvent" /> to a <see cref="EventHorizonEvent" />.
        /// </summary>
        /// <param name="committedEvent">The <see cref="CommittedEvent" />.</param>
        /// <param name="streamPosition">The <see cref="StreamPosition" />.</param>
        /// <param name="eventHorizon">The <see cref="Runtime.EventHorizon.EventHorizon" />.</param>
        /// <returns>The converted <see cref="EventHorizonEvent" />.</returns>
        public static EventHorizonEvent ToNewEventHorizonEvent(this CommittedEvent committedEvent, StreamPosition streamPosition, Runtime.EventHorizon.EventHorizon eventHorizon) =>
            new EventHorizonEvent(
                streamPosition,
                committedEvent.GetEventHorizonEventMetadata(eventHorizon),
                BsonDocument.Parse(committedEvent.Content));
    }
}